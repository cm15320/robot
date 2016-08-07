﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Linq;

using NatNetML;
using System.Xml.Serialization;

namespace robotTracking
{   
    public partial class RobotTracker : Form
    {
        public RobotTracker()
        {
            InitializeComponent();
            ConnectedToRobotOutcome += robotConnectResult;
        }

        public delegate void connectedRobotEventHandler(object source, EventArgs args);
        public event connectedRobotEventHandler ConnectedToRobotOutcome;

        private delegate void RobotConnectResultCallback(object sender, EventArgs e);





        private NatNetML.NatNetClientML m_NatNet;
        private bool connected = false;

        private RobotControl controller = new RobotControl();
        private bool connectedRobot = false;
        private bool connectingToRobot = false;

        private Experiment experiment;
        private bool experimentRunning = false;
        

        // The current frame of mocap date
        private NatNetML.FrameOfMocapData m_CurrentFrameOfData = new NatNetML.FrameOfMocapData();

        NatNetML.ServerDescription serverDesc = new NatNetML.ServerDescription();

        // A queue to hold the incoming frame of data objects
        private Queue<NatNetML.FrameOfMocapData> m_FrameQueue = new Queue<NatNetML.FrameOfMocapData>();

        // Hashtables to store the markers, rigid bodies and skeletons (may make just Lists)
        Hashtable htMarkers = new Hashtable();
        Hashtable htRigidBodies = new Hashtable();
        List<NatNetML.RigidBody> mRigidBodies = new List<RigidBody>();
        Hashtable htSkelRBs = new Hashtable();

        // possibly add graph stuff here in future

        // frame timing information
        double m_fLastFrameTimestamp = 0.0f;
        float m_fCurrentMocapFrameTimestamp = 0.0f;
        float m_fFirstMocapFrameTimestamp = 0.0f;
        QueryPerfCounter m_FramePeriodTimer = new QueryPerfCounter();

        // The UI here for now will just mean writing info to the console 
        QueryPerfCounter m_UIUpdateTimer = new QueryPerfCounter();

        // server info
        double m_ServerFramerate = 1.0f;
        float m_ServerToMillimeters = 1.0f;
        int m_UpAxis = 1;   // 0=x, 1=y, 2=z (Y default)
        int mAnalogSamplesPerMocpaFrame = 0;
        int mDroppedFrames = 0;
        int mLastFrame = 0;

        private string strLocalIP, strServerIP;

        private static object syncLock = new object();
        private bool needMarkerListUpdate = false;
        private bool mPaused = false;

        private delegate void OutputMessageCallback(string strMessage);


        // UI updating
        delegate void UpdateUICallback();
        bool mApplicationRunning = true;
        // May not be as critical when printing to the console but 
        // will operate on separate thread to those in charge of updating values etc.
        Thread UIUpdateThread;

        // polling info (for actively grabbing information from the server)
        delegate void PollCallback();
        Thread pollThread;
        bool mPolling = false;

        // recording info goes here if required

        private void Form1_Load(object sender, EventArgs e)
        {
            setupConnection();

        }



        /* Normally this would be where different ip addresses could be entered etc for the connection,
         * instead we will just use the localhost ip address 127.0.0.1
         */
        private void setupConnection()
        {
            strLocalIP = "127.0.0.1";
            strServerIP = strLocalIP;

            // create the NatNet Client
            int iConnectionType = 0; // we use 0 for multicast (1 is unicast)
            int iResult = CreateClient(iConnectionType);

            // create and run an updateUI thread that will call the updateUI callback every 15ms
            UpdateUICallback updateUICB = new UpdateUICallback(UpdateUI); //UpdateUI is the callback method
            UIUpdateThread = new Thread(() =>
            {
                while (mApplicationRunning)
                {
                    try
                    {
                        this.Invoke(updateUICB);
                        Thread.Sleep(15);
                    }
                    catch (System.Exception ex)
                    {
                        OutputMessage(ex.Message);
                        break;
                    }
                }
            });

            // start this thread going
            UIUpdateThread.Start();

            // create and run a polling thread that will poll the data every 15ms
            PollCallback pollCB = new PollCallback(PollData); //PollData is the callback fn
            pollThread = new Thread(() =>
            {
                while (mPolling)
                {
                    try
                    {
                        this.Invoke(pollCB);
                        Thread.Sleep(15);
                    }
                    catch (System.Exception ex)
                    {
                        OutputMessage(ex.Message);
                        break;
                    }
                }
            });
        }

        // creates a NatNet client based on the connection type
        private int CreateClient(int iConnectionType)
        {
            // release any previous clients
            if (m_NatNet != null)
            {
                m_NatNet.Uninitialize();
            }

            m_NatNet = new NatNetML.NatNetClientML(iConnectionType);

            

            // set a "frame ready" callback fn (event handler) that will be called by
            // NatNet when it receives a frame of data from the server application
            m_NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);
            // nb the m_NatNet_onFrameReady fn is the callback defined later

            // print the version info
            int[] ver = new int[4];
            ver = m_NatNet.NatNetVersion();
            String strVersion = String.Format("NatNet Version: {0}.{1}.{2}.{3}", ver[0], ver[1], ver[2], ver[3]);
            OutputMessage(strVersion);
            Console.WriteLine(strVersion);

            return 0;

        }

        private void OutputMessage(string strMessage)
        {
            if (mPaused)
                return;

            if (!mApplicationRunning)
                return;

            if (this.listView1.InvokeRequired)
            {
                // It's on a different thread, so use Invoke
                OutputMessageCallback d = new OutputMessageCallback(OutputMessage);
                this.Invoke(d, new object[] { strMessage });
            }
            else
            {
                // It's on the same thread, no need for Invoke
                DateTime d = DateTime.Now;
                String strTime = String.Format("{0}:{1}:{2}:{3}", d.Hour, d.Minute, d.Second, d.Millisecond);
                ListViewItem item = new ListViewItem(strTime, 0);
                item.SubItems.Add(strMessage);
                listView1.Items.Add(item);
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(!connected)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }

        }


        // Connects to a NatNet server (Motive in this case)
        private void Connect()
        {
            int returnCode = 0;

            returnCode = m_NatNet.Initialize(strLocalIP, strServerIP);
            if (returnCode == 0)
            {
                OutputMessage("Initialization succeeded");
            }
            else
            {
                OutputMessage("Error Initializing");
            }

            // validate the connection
            returnCode = m_NatNet.GetServerDescription(serverDesc);
            if (returnCode == 0)
            {
                Console.WriteLine("Connection succeeded");
                Console.WriteLine("server app name: " + serverDesc.HostApp);
                Console.WriteLine(string.Format("   Server App Version: {0}.{1}.{2}.{3}", serverDesc.HostAppVersion[0], serverDesc.HostAppVersion[1], serverDesc.HostAppVersion[2], serverDesc.HostAppVersion[3]));
                Console.WriteLine(String.Format("   Server NatNet Version: {0}.{1}.{2}.{3}", serverDesc.NatNetVersion[0], serverDesc.NatNetVersion[1], serverDesc.NatNetVersion[2], serverDesc.NatNetVersion[3]));

                // Tracking Tools and Motive report in meters so convert to mm
                if (serverDesc.HostApp.Contains("TrackingTools") || serverDesc.HostApp.Contains("Motive"))
                    m_ServerToMillimeters = 1000.0f;

                //  could query for current camera frame rate and analog framereate here

                // [NatNet] [optional] Query mocap server for the current up axis
                int nBytes = 0;
                byte[] response = new byte[10000];
                int rc = m_NatNet.SendMessageAndWait("UpAxis", out response, out nBytes);
                if (rc == 0)
                {
                    m_UpAxis = BitConverter.ToInt32(response, 0);
                }

                // [NatNet] [optional] Query mocap server for the current camera framerate
                nBytes = 0;
                response = new byte[10000];
                rc = m_NatNet.SendMessageAndWait("FrameRate", out response, out nBytes);
                if (rc == 0)
                {
                    try
                    {
                        m_ServerFramerate = BitConverter.ToSingle(response, 0);
                        OutputMessage(String.Format("   Camera Framerate: {0}", m_ServerFramerate));
                    }
                    catch (System.Exception ex)
                    {
                        OutputMessage(ex.Message);
                    }
                }



                // initialise timestamp info
                m_fCurrentMocapFrameTimestamp = 0.0f;
                m_fFirstMocapFrameTimestamp = 0.0f;
                mDroppedFrames = 0;

                connected = true;
                connectButton.Text = "Disconnect";
                getDataDescriptions();
                onConnectRobotAttempt();
            }
            else
            {
                OutputMessage("Error connecting");
            }

        }

        private RigidBody findRigidBody(int id, int parentID = -2)
        {
            foreach (RigidBody rb in mRigidBodies)
            {
                if (rb.ID == id)
                {
                    if (parentID != -2)
                    {
                        if (rb.parentID == parentID)
                            return rb;
                    }
                    else
                    {
                        return rb;
                    }
                }
            }
            return null;
        }

        private void Disconnect()
        {
            int nBytes = 0;
            byte[] response = new byte[10000];
            int rc;
            rc = m_NatNet.SendMessageAndWait("Disconnect", out response, out nBytes);
            if (rc == 0)
            {

            }
            // shutdown our client socket
            m_NatNet.Uninitialize();
            connected = false;
            connectButton.Text = "Connect";
        }



        private void SetDataPolling(bool poll)
        {
            if (poll)
            {
                // disable event based data handling
                m_NatNet.OnFrameReady -= m_NatNet_OnFrameReady;

                // enable polling 
                mPolling = true;
                pollThread.Start();
            }
            else
            {
                // disable polling
                mPolling = false;

                // enable event based data handling
                m_NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);
            }
        }

        // This method, unlike with the original natnet version, is used at the start in order
        // to get the rigid body descriptions into the mRigidBodies list so that they can then
        // be used to check for the required rigid body names (robotBase and robotTip)
        private void getDataDescriptions()
        {
            mRigidBodies.Clear();
            dataGridView1.Rows.Clear();
            htMarkers.Clear();
            htRigidBodies.Clear();
            htSkelRBs.Clear();
            //needMarkerListUpdate = true;

            List<NatNetML.DataDescriptor> descs = new List<NatNetML.DataDescriptor>();
            bool bSuccess = m_NatNet.GetDataDescriptions(out descs);
            if (bSuccess)
            {
                OutputMessage(String.Format("Retrieved {0} Data Descriptions....", descs.Count));
                int iObject = 0;
                foreach (NatNetML.DataDescriptor d in descs)
                {
                    iObject++;

                    // MarkerSets
                    if (d.type == (int)NatNetML.DataDescriptorType.eMarkerSetData)
                    {
                        NatNetML.MarkerSet ms = (NatNetML.MarkerSet)d;
                        OutputMessage("Data Def " + iObject.ToString() + " [MarkerSet]");

                        OutputMessage(" Name : " + ms.Name);
                        OutputMessage(String.Format(" Markers ({0}) ", ms.nMarkers));
                        dataGridView1.Rows.Add("MarkerSet: " + ms.Name);
                        for (int i = 0; i < ms.nMarkers; i++)
                        {
                            OutputMessage(("  " + ms.MarkerNames[i]));
                            int rowIndex = dataGridView1.Rows.Add("  " + ms.MarkerNames[i]);
                            // MarkerNameIndexToRow map
                            String strUniqueName = ms.Name + i.ToString();
                            int key = strUniqueName.GetHashCode();
                            htMarkers.Add(key, rowIndex);
                        }
                    }
                    // RigidBodies
                    else if (d.type == (int)NatNetML.DataDescriptorType.eRigidbodyData)
                    {
                        NatNetML.RigidBody rb = (NatNetML.RigidBody)d;

                        OutputMessage("Data Def " + iObject.ToString() + " [RigidBody]");
                        OutputMessage(" Name : " + rb.Name);
                        OutputMessage(" ID : " + rb.ID);
                        OutputMessage(" ParentID : " + rb.parentID);
                        OutputMessage(" OffsetX : " + rb.offsetx);
                        OutputMessage(" OffsetY : " + rb.offsety);
                        OutputMessage(" OffsetZ : " + rb.offsetz);

                        mRigidBodies.Add(rb);

                        int rowIndex = dataGridView1.Rows.Add("RigidBody: " + rb.Name);
                        // RigidBodyIDToRow map
                        int key = rb.ID.GetHashCode();
                        try
                        {
                            htRigidBodies.Add(key, rowIndex);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Duplicate RigidBody ID Detected : " + ex.Message);
                        }

                    }
                    // Skeletons
                    else if (d.type == (int)NatNetML.DataDescriptorType.eSkeletonData)
                    {
                        NatNetML.Skeleton sk = (NatNetML.Skeleton)d;

                        OutputMessage("Data Def " + iObject.ToString() + " [Skeleton]");
                        OutputMessage(" Name : " + sk.Name);
                        OutputMessage(" ID : " + sk.ID);
                        dataGridView1.Rows.Add("Skeleton: " + sk.Name);
                        for (int i = 0; i < sk.nRigidBodies; i++)
                        {
                            RigidBody rb = sk.RigidBodies[i];
                            OutputMessage(" RB Name  : " + rb.Name);
                            OutputMessage(" RB ID    : " + rb.ID);
                            OutputMessage(" ParentID : " + rb.parentID);
                            OutputMessage(" OffsetX  : " + rb.offsetx);
                            OutputMessage(" OffsetY  : " + rb.offsety);
                            OutputMessage(" OffsetZ  : " + rb.offsetz);

                            //mRigidBodies.Add(rb);
                            int key = sk.ID * 1000 + rb.ID;
                            htSkelRBs.Add(key, rb);

                        }
                    }
                    else
                    {
                        OutputMessage("Unknown DataType");
                    }
                }
            }
            else
            {
                OutputMessage("Unable to retrieve DataDescriptions");
            }
        }



        // This is a key function that updates the display of the data 
        // It is called by the update UI function

        private void updateData()
        {
            // loop through the marker set data
            for (int i = 0; i < m_CurrentFrameOfData.nMarkerSets; i++)
            {
                NatNetML.MarkerSetData ms = m_CurrentFrameOfData.MarkerSets[i];
                // loop through each marker in the set
                for (int j = 0; j < ms.nMarkers; j++)
                {
                    string strUniqueName = ms.MarkerSetName + j.ToString();
                    // get hashcode from the unique marker name
                    int key = strUniqueName.GetHashCode();
                    if (htMarkers.Contains(key))
                    {
                        int rowIndex = (int)htMarkers[key];
                        if (rowIndex >= 0)
                        {
                            dataGridView1.Rows[rowIndex].Cells[1].Value = ms.Markers[j].x;
                            dataGridView1.Rows[rowIndex].Cells[2].Value = ms.Markers[j].y;
                            dataGridView1.Rows[rowIndex].Cells[3].Value = ms.Markers[j].z;
                        }

                        //Console.WriteLine(string.Format("{0} coords: x = {1}, y = {2}, z = {3}", strUniqueName, x, y, z));
                    }
                }
            }

            // loop through RigidBody data
            for (int i = 0; i < m_CurrentFrameOfData.nRigidBodies; i++)
            {
                NatNetML.RigidBodyData rb = m_CurrentFrameOfData.RigidBodies[i];
                // get the hashcode of the id for later displaying in grid form
                int key = rb.ID.GetHashCode();

                // note : must add rb definitions here one time instead of on get data descriptions because we don't know the marker list yet.
                if (!htRigidBodies.ContainsKey(key))
                {
                    // add rigidbody def to the grid
                    if ((rb.Markers[0] != null) && (rb.Markers[0].ID != -1))
                    {
                        string name;
                        RigidBody rbdef = findRigidBody(rb.ID);
                        if (rbdef != null)
                        {
                            name = rbdef.Name;
                        }
                        else
                        {
                            name = rb.ID.ToString();
                        }

                        // add to the rigid bodies hash table, the key based on the hash code of the id versus the row index
                        int rowIndex = dataGridView1.Rows.Add("RigidBody: " + name);
                        key = rb.ID.GetHashCode();
                        htRigidBodies.Add(key, rowIndex);

                        //add markers associated with this rigid body to the grid

                        for (int j = 0; j < rb.nMarkers; j++)
                        {
                            String strUniqueName = name + "-" + rb.Markers[j].ID.ToString();
                            int keyMarker = strUniqueName.GetHashCode();
                            int newRowIndexMarker = dataGridView1.Rows.Add(strUniqueName);
                            htMarkers.Add(keyMarker, newRowIndexMarker);
                        }
                    }
                }
                else
                {
                    // update the rigid body data printed/displayed as must already have it in table


                    int rowIndex = (int)htRigidBodies[key];
                    if (rowIndex >= 0) // check if it is in hash table
                    {
                        bool tracked = rb.Tracked;
                        if (!tracked)
                        {
                            OutputMessage("a rigid body is not tracked in this frame");
                        }

                        //dataGridView1.Rows[rowIndex].Cells[1].Value = rb.x * m_ServerToMillimeters;
                        //dataGridView1.Rows[rowIndex].Cells[2].Value = rb.y * m_ServerToMillimeters;
                        //dataGridView1.Rows[rowIndex].Cells[3].Value = rb.z * m_ServerToMillimeters;


                        //// Convert quaternion to eulers.  Motive coordinate conventions: X(Pitch), Y(Yaw), Z(Roll), Relative, RHS
                        //float[] quaternion = new float[4] { rb.qx, rb.qy, rb.qz, rb.qw };
                        //float[] eulers = new float[3];
                        //eulers = m_NatNet.QuatToEuler(quaternion, (int)NATEulerOrder.NAT_XYZr);

                        //double xdeg = RadiansToDegrees(eulers[0]);
                        //double ydeg = RadiansToDegrees(eulers[1]);
                        //double zdeg = RadiansToDegrees(eulers[2]);

                        //dataGridView1.Rows[rowIndex].Cells[4].Value = xdeg;
                        //dataGridView1.Rows[rowIndex].Cells[5].Value = ydeg;
                        //dataGridView1.Rows[rowIndex].Cells[6].Value = zdeg;

                        updateRigidBodyData(rb, key);

                        // now must update the marker data of those tied to this rigid body

                        //for (int j = 0; j < rb.nMarkers; j++)
                        //{
                        //    if (rb.Markers[j].ID != -1)
                        //    {
                        //        string name;
                        //        RigidBody rbDef = findRigidBody(rb.ID);
                        //        if (rbDef != null)
                        //        {
                        //            name = rbDef.Name;
                        //        }
                        //        else
                        //        {
                        //            name = rb.ID.ToString();
                        //        }

                        //        String strUniqueName = name + "-" + rb.Markers[j].ID.ToString();
                        //        int keyMarker = strUniqueName.GetHashCode();
                        //        if (htMarkers.ContainsKey(keyMarker))
                        //        {
                        //            int rowIndexMarker = (int)htMarkers[keyMarker];
                        //            NatNetML.Marker m = rb.Markers[j];
                        //            dataGridView1.Rows[rowIndexMarker].Cells[1].Value = m.x;
                        //            dataGridView1.Rows[rowIndexMarker].Cells[2].Value = m.y;
                        //            dataGridView1.Rows[rowIndexMarker].Cells[3].Value = m.z;
                        //        }

                        //    }
                        //}

                        RigidBody rbDef = findRigidBody(rb.ID);
                        updateRigidBodyMarkerData(rb, rbDef);

                    }

                }
            }

            // update Skeleton data if there is any (collection of rigid bodies)
            for (int i = 0; i < m_CurrentFrameOfData.nSkeletons; i++)
            {
                NatNetML.SkeletonData sk = m_CurrentFrameOfData.Skeletons[i];
                for (int j = 0; j < sk.nRigidBodies; j++)
                {
                    // note : skeleton rigid body ids are of the form:
                    // parent skeleton ID   : high word (upper 16 bits of int)
                    // rigid body id        : low word  (lower 16 bits of int)
                    NatNetML.RigidBodyData rb = sk.RigidBodies[j];
                    int skeletonID = HighWord(rb.ID);
                    int rigidBodyID = LowWord(rb.ID);
                    int uniqueID = skeletonID * 1000 + rigidBodyID;
                    int key = uniqueID.GetHashCode();

                    // note : must add rb definitions here one time instead of on get data descriptions because we don't know the marker list yet.
                    if (!htRigidBodies.ContainsKey(key))
                    {
                        // Add RigidBody def to the grid
                        if ((rb.Markers[0] != null) && (rb.Markers[0].ID != -1))
                        {
                            int key1 = sk.ID * 1000 + rigidBodyID;
                            RigidBody rbDef = (RigidBody)htSkelRBs[key1];
                            if (rbDef != null)
                            {
                                int rowIndex = dataGridView1.Rows.Add("Bone: " + rbDef.Name);
                                htRigidBodies.Add(key, rowIndex);
                                // Add Markers associated with this rigid body to the grid
                                for (int k = 0; k < rb.nMarkers; k++)
                                {
                                    String strUniqueName = rbDef.Name + "-" + rb.Markers[k].ID.ToString();
                                    int keyMarker = strUniqueName.GetHashCode();
                                    int newRowIndexMarker = dataGridView1.Rows.Add(strUniqueName);
                                    htMarkers.Add(keyMarker, newRowIndexMarker);
                                }

                            }
                        }
                    }
                    else
                    {
                        int rowIndex = (int)htRigidBodies[key];
                        if (rowIndex >= 0)
                        {
                            //dataGridView1.Rows[rowIndex].Cells[1].Value = rb.x;
                            //dataGridView1.Rows[rowIndex].Cells[2].Value = rb.y;
                            //dataGridView1.Rows[rowIndex].Cells[3].Value = rb.z;

                            //// Convert quaternion to eulers.  Motive coordinate conventions: X(Pitch), Y(Yaw), Z(Roll), Relative, RHS
                            //float[] quat = new float[4] { rb.qx, rb.qy, rb.qz, rb.qw };
                            //float[] eulers = new float[3];
                            //eulers = m_NatNet.QuatToEuler(quat, (int)NATEulerOrder.NAT_XYZr);
                            //double x = RadiansToDegrees(eulers[0]);     // convert to degrees
                            //double y = RadiansToDegrees(eulers[1]);
                            //double z = RadiansToDegrees(eulers[2]);

                            //dataGridView1.Rows[rowIndex].Cells[4].Value = x;
                            //dataGridView1.Rows[rowIndex].Cells[5].Value = y;
                            //dataGridView1.Rows[rowIndex].Cells[6].Value = z;

                            updateRigidBodyData(rb, key);

                            // Marker data associated with this rigid body
                            int key1 = sk.ID * 1000 + rigidBodyID;
                            RigidBody rbDef = (RigidBody)htSkelRBs[key1];
                            if (rbDef != null)
                            {
                                //for (int k = 0; k < rb.nMarkers; k++)
                                //{
                                //    String strUniqueName = rbDef.Name + "-" + rb.Markers[k].ID.ToString();
                                //    int keyMarker = strUniqueName.GetHashCode();
                                //    if (htMarkers.ContainsKey(keyMarker))
                                //    {
                                //        int rowIndexMarker = (int)htMarkers[keyMarker];
                                //        NatNetML.Marker m = rb.Markers[k];
                                //        dataGridView1.Rows[rowIndexMarker].Cells[1].Value = m.x;
                                //        dataGridView1.Rows[rowIndexMarker].Cells[2].Value = m.y;
                                //        dataGridView1.Rows[rowIndexMarker].Cells[3].Value = m.z;
                                //    }
                                //}
                                updateRigidBodyMarkerData(rb, rbDef);
                            }
                        }
                    }
                }
            }   // end skeleton update

            // force plate data would go here

            // also lone markers would be updated here


        }

        public void updateRigidBodyData(NatNetML.RigidBodyData rb, int key)
        {
            int rowIndex = (int)htRigidBodies[key];
            if (rowIndex >= 0)
            {
                dataGridView1.Rows[rowIndex].Cells[1].Value = rb.x * m_ServerToMillimeters;
                dataGridView1.Rows[rowIndex].Cells[2].Value = rb.y * m_ServerToMillimeters;
                dataGridView1.Rows[rowIndex].Cells[3].Value = rb.z * m_ServerToMillimeters;

                // Convert quaternion to eulers.  Motive coordinate conventions: X(Pitch), Y(Yaw), Z(Roll), Relative, RHS
                float[] quat = new float[4] { rb.qx, rb.qy, rb.qz, rb.qw };
                float[] eulers = new float[3];
                eulers = m_NatNet.QuatToEuler(quat, (int)NATEulerOrder.NAT_XYZr);
                double x = RadiansToDegrees(eulers[0]);     // convert to degrees
                double y = RadiansToDegrees(eulers[1]);
                double z = RadiansToDegrees(eulers[2]);

                dataGridView1.Rows[rowIndex].Cells[4].Value = x;
                dataGridView1.Rows[rowIndex].Cells[5].Value = y;
                dataGridView1.Rows[rowIndex].Cells[6].Value = z;

            }
        }


        public void updateRigidBodyMarkerData(NatNetML.RigidBodyData rb, NatNetML.RigidBody rbDef)
        {
            if (rbDef != null)
            {
                for (int k = 0; k < rb.nMarkers; k++)
                {
                    String strUniqueName = rbDef.Name + "-" + rb.Markers[k].ID.ToString();
                    int keyMarker = strUniqueName.GetHashCode();
                    if (htMarkers.ContainsKey(keyMarker))
                    {
                        int rowIndexMarker = (int)htMarkers[keyMarker];
                        NatNetML.Marker m = rb.Markers[k];
                        dataGridView1.Rows[rowIndexMarker].Cells[1].Value = m.x;
                        dataGridView1.Rows[rowIndexMarker].Cells[2].Value = m.y;
                        dataGridView1.Rows[rowIndexMarker].Cells[3].Value = m.z;
                    }
                }
            }

        }



        public void UpdateUI()
        {
            m_UIUpdateTimer.Stop();
            double interframeDuration = m_UIUpdateTimer.Duration();

            QueryPerfCounter uiIntraFrameTimer = new QueryPerfCounter();
            uiIntraFrameTimer.Start();

            // the frame queue is a shared resource with the FrameOfMocap delivery thread, so lock it while reading
            // note this can block the frame delivery thread.  In a production application frame queue management would be optimized.


            lock (syncLock)
            {
                while (m_FrameQueue.Count > 0)
                {
                    m_CurrentFrameOfData = m_FrameQueue.Dequeue();

                    if (m_FrameQueue.Count > 0)
                        continue;

                    if (m_CurrentFrameOfData != null)
                    {
                        // for servers that only use timestamps, not frame numbers, calculate a 
                        // frame number from the time delta between frames
                        if (serverDesc.HostApp.Contains("TrackingTools"))
                        {
                            m_fCurrentMocapFrameTimestamp = m_CurrentFrameOfData.fLatency;
                            if (m_fCurrentMocapFrameTimestamp == m_fLastFrameTimestamp)
                            {
                                continue;
                            }
                            if (m_fFirstMocapFrameTimestamp == 0.0f)
                            {
                                m_fFirstMocapFrameTimestamp = m_fCurrentMocapFrameTimestamp;
                            }
                            m_CurrentFrameOfData.iFrame = (int)((m_fCurrentMocapFrameTimestamp - m_fFirstMocapFrameTimestamp) * m_ServerFramerate);

                        }

                        // update the data
                        updateData();

                        // timing information
                        uiIntraFrameTimer.Stop();
                        double uiIntraFrameDuration = uiIntraFrameTimer.Duration();
                        m_UIUpdateTimer.Start();

                        // experiment stages
                        if(experimentRunning)
                        {
                            experiment.update(m_CurrentFrameOfData);
                        }
                    }
                }
            }

        }




        public void PollData()
        {
            FrameOfMocapData data = m_NatNet.GetLastFrameOfData();
            ProcessFrameOfData(ref data);

        }


        public double RadiansToDegrees(double rad)
        {
            return rad * (180.0f / Math.PI);

        }


        // The callback for the NatNet server to call when a frame is ready to send
        void m_NatNet_OnFrameReady(NatNetML.FrameOfMocapData data, NatNetML.NatNetClientML client)
        {
            //Console.WriteLine("frame sent");
            double elapsedIntraMS = 0.0f;
            QueryPerfCounter intraTimer = new QueryPerfCounter();
            intraTimer.Start();

            // detect and report and 'measured' frame drop (as measured by client)
            m_FramePeriodTimer.Stop();
            double elapsedMS = m_FramePeriodTimer.Duration();

            ProcessFrameOfData(ref data);

            // report if we are taking too long, which blocks packet receiving, which if long enough would result in socket buffer drop
            intraTimer.Stop();
            elapsedIntraMS = intraTimer.Duration();
            if (elapsedIntraMS > 5.0f)
            {
                Console.WriteLine("Warning : Frame handler taking too long: " + elapsedIntraMS.ToString("F2"));
            }

            m_FramePeriodTimer.Start();

        }


        private void pollDataCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            bool isPoll = pollDataCheckbox.Checked;
            SetDataPolling(isPoll);
        }

        private void RobotTracker_FormClosing(object sender, FormClosingEventArgs e)
        {
            mApplicationRunning = false;

            if (UIUpdateThread.IsAlive)
                UIUpdateThread.Abort();

            m_NatNet.Uninitialize();

        }


        // Process the frame of data in temrs of the timestamp etc to check frames dropped
        // can optionally clear the queue each time one is processed 
        void ProcessFrameOfData(ref NatNetML.FrameOfMocapData data)
        {
            // detect and reported any 'reported' frame drop (as reported by server)
            if (m_fLastFrameTimestamp != 0.0f)
            {
                double framePeriod = 1.0f / m_ServerFramerate;
                double thisPeriod = data.fTimestamp - m_fLastFrameTimestamp;
                double fudgeFactor = 0.002f; // 2 ms
                if ((thisPeriod - framePeriod) > fudgeFactor)
                {
                    //OutputMessage("Frame Drop: ( ThisTS: " + data.fTimestamp.ToString("F3") + "  LastTS: " + m_fLastFrameTimestamp.ToString("F3") + " )");
                    mDroppedFrames++;
                }
            }

            // check and report frame drop (frame id based)
            if (mLastFrame != 0)
            {
                if ((data.iFrame - mLastFrame) != 1)
                {
                    //OutputMessage("Frame Drop: ( ThisFrame: " + data.iFrame.ToString() + "  LastFrame: " + mLastFrame.ToString() + " )");
                    //mDroppedFrames++;
                }
            }

            // recording : write packet to data file
            //if (mRecording)
            //{
            //    WriteFrame(data);
            //}

            // [NatNet] Add the incoming frame of mocap data to our frame queue,  
            // Note: the frame queue is a shared resource with the UI thread, so lock it while writing
            lock (syncLock)
            {
                // [optional] clear the frame queue before adding a new frame
                //m_FrameQueue.Clear();
                FrameOfMocapData deepCopy = new FrameOfMocapData(data);
                m_FrameQueue.Enqueue(deepCopy);
            }

            mLastFrame = data.iFrame;
            m_fLastFrameTimestamp = data.fTimestamp;

        }

        public int LowWord(int number)
        {
            return number & 0xFFFF;
        }


        protected virtual void onConnectRobotAttempt()
        {

            if(ConnectedToRobotOutcome != null)
            {
                ConnectedToRobotOutcome(this, EventArgs.Empty);
            }
        }

        private void attemptRobotConnect()
        {
            connectedRobot = controller.initialise();
            if (!connectedRobot) OutputMessage("Error connecting to robot");
            onConnectRobotAttempt();
        }

        private void robotConnectResult(object sender, EventArgs e)
        {

            // if on a different thread then must deal with it from main thread
            if(connectRobotButton.InvokeRequired)
            {
                RobotConnectResultCallback d = new RobotConnectResultCallback(robotConnectResult);
                this.Invoke(d, new object[] {this, EventArgs.Empty });
                return;
            }

            connectingToRobot = false;
            connectRobotButton.Enabled = true;

            if(connectedRobot)
            {
                robotConnectLabel.Text = "Connected";
                connectRobotButton.Text = "Disconnect robot";
                testMovementButton.Enabled = true;
                experimentButton.Enabled = true;
                getDataDescriptions();
                if (connected)  runCalibrationButton.Enabled = true;
            }
            else
            {
                robotConnectLabel.Text = "Not connected";
                connectRobotButton.Text = "Connect robot";
            }
        }

        private void connectRobotButton_Click(object sender, EventArgs e)
        {
            if(!connectedRobot && !connectingToRobot)
            {
                // make new thread to connect to arduino port with a callback for when it is done that changes the button

                robotConnectLabel.Text = "Connecting...";
                connectRobotButton.Enabled = false;
                connectingToRobot = true;
                
                
                new Task(attemptRobotConnect).Start();
            }
            else if(connectedRobot && !connectingToRobot)
            {
                controller.uninitialise();
                experimentButton.Enabled = false;
                connectRobotButton.Text = "Connect";
                robotConnectLabel.Text = "Not Connected";
            }
        }


        private void testMovementButton_Click(object sender, EventArgs e)
        {
            if(connectedRobot)  controller.test();
        }


        private void experimentButton_Click(object sender, EventArgs e)
        {
            if(requiredObjectsTracked() && !experimentRunning && connectedRobot)
            {
                experimentRunning = true;
                experiment = new Experiment(controller, mRigidBodies);
                experimentButton.Text = "Stop experiment";
            }
            else if (experimentRunning)
            {
                experimentRunning = false;
                experimentButton.Text = "Start experiment";
            }
            else if (!requiredObjectsTracked())
            {
                OutputMessage("Cannot locate all the required bodies (robotBase and robotTip)");
            }
        }


        private bool requiredObjectsTracked()
        {
            int cnt = 0;
            string[] requiredObjects = new string[] { "robotBase", "robotTip" };
            foreach(RigidBody rb in mRigidBodies)
            {
                if (requiredObjects.Contains(rb.Name)) cnt++;
            }

            if(cnt == requiredObjects.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void buttonTestStorage_Click(object sender, EventArgs e)
        {
            float[] motorAngles = new float[4] { 12.5f, 31.4f, 24.5f, 54.1f };
            float[] tipPos = new float[3] { 13.4f, 51.5f, 145f };
            float[] tipAngle = new float[3] { 43.4f, 35f, 255f };

            float[] motorAngles2 = new float[4] { 22.5f, 21.4f, 24.5f, 24.1f };
            float[] tipPos2 = new float[3] { 23.4f, 21.5f, 245f };
            float[] tipAngle2 = new float[3] { 23.4f, 25f, 255f };


            CalibrationData testData = new CalibrationData();

            DataPoint testDataPoint1 = new DataPoint();
            testDataPoint1.setMotorAngles(motorAngles);
            testDataPoint1.setTipPos(tipPos);
            testDataPoint1.setTipOrientation(tipAngle);

            DataPoint testDataPoint2 = new DataPoint();
            testDataPoint2.setMotorAngles(motorAngles2);
            testDataPoint2.setTipPos(tipPos2);
            testDataPoint2.setTipOrientation(tipAngle2);

            testData.Add(testDataPoint1);
            testData.Add(testDataPoint2);


            XmlSerializer ser = new XmlSerializer(typeof(CalibrationData));
            string filename = "calibrationData.xml";
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, testData);




            Console.WriteLine("should have written data.xml");

            writer.Close();

        }

        private void buttonTestRetrieval_Click(object sender, EventArgs e)
        {
            XmlSerializer reader = new XmlSerializer(typeof(CalibrationData));
            StreamReader file = new StreamReader("calibrationData.xml");
            CalibrationData testData = (CalibrationData)reader.Deserialize(file);
            file.Close();
            for(int i = 0; i < testData.Count; i++)
            {
                Console.WriteLine("for data point number " + i);
                Console.WriteLine("motor angles:");
                for(int j = 0; j < testData[i].motorAngles.Length; j++)
                {
                    Console.Write(testData[i].motorAngles[j] + "   ");
                }
                Console.WriteLine();
            }


        }

        public int HighWord(int number)
        {
            return ((number >> 16) & 0xFFFF);
        }



    }

    [Serializable]
    public class CalibrationData : ICollection
    {
        [XmlArrayAttribute("tableData")]
        private List<DataPoint> tableData = new List<DataPoint>();

        public DataPoint this[int index]
        {
            get { return (DataPoint)tableData[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            Array tableDataArray = tableData.ToArray();
            tableDataArray.CopyTo(a, index);
        }

        public int Count
        {
            get { return tableData.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            return tableData.GetEnumerator();
        }

        public void Add(DataPoint newDataPoint)
        {
            tableData.Add(newDataPoint);
        }

    }

    public class QueryPerfCounter
    {
        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long start;
        private long stop;
        private long frequency;
        Decimal multiplier = new Decimal(1.0e9);

        public QueryPerfCounter()
        {
            if (QueryPerformanceFrequency(out frequency) == false)
            {
                // Frequency not supported
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            QueryPerformanceCounter(out start);
        }

        public void Stop()
        {
            QueryPerformanceCounter(out stop);
        }

        // return elapsed time between start and stop, in milliseconds.
        public double Duration()
        {
            double val = ((double)(stop - start) * (double)multiplier) / (double)frequency;
            val = val / 1000000.0f;   // convert to ms
            return val;
        }
    }

}