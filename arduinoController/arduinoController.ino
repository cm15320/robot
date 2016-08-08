#include <Servo.h>

int onBoardLed = 13;

byte inputBytes[5];
bool connectedToPC = false;
Servo myservo1, myservo2, myservo3, myservo4;
Servo currentServo = myservo1;

void setup() {
  // Set up pins etc
  pinMode(onBoardLed, OUTPUT);
  Serial.begin(57600);
  digitalWrite(onBoardLed, LOW);
  myservo1.attach(9);
  myservo2.attach(10);
  myservo3.attach(11);
  myservo4.attach(6);
}

void loop() {
  // Read buffer input bytes
  if(!connectedToPC) {
    checkHandshake();
  }

  if(sentAllAngles()) {
    int angleM1 = Serial.read();
    int angleM2 = Serial.read();
    int angleM3 = Serial.read();
    int angleM4 = Serial.read();

    currentServo = myservo1;
    currentServo.write(angleM1);
    delay(200);
    currentServo = myservo2;
    currentServo.write(angleM2);
    delay(200);
    currentServo = myservo3;
    currentServo.write(angleM3);
    delay(200);
    currentServo = myservo4;
    currentServo.write(angleM4);

    
  }

  else if (sentFromPC()) {
    int selectedServo = Serial.read();
    int angle = Serial.read();

    switch(selectedServo) {
      case 1:
//        myservo1.attach(9);
        currentServo = myservo1;
        break;
      case 2:
//        myservo2.attach(10);
        currentServo = myservo2;
        break;
      case 3:
//        myservo3.attach(11);
        currentServo = myservo3;
        break;
      case 4:
//        myservo4.attach(6);
        currentServo = myservo4;
        break;
      default:
        break;
    }

    currentServo.write(angle);
//    delay(200);
//    currentServo.detach();
  }

  else if(userEntered()) {
//    if(Serial.available() == 1) {
      int receivedInt = Serial.read();
      char receivedChar = (char)receivedInt;
      if(receivedInt == 1) {
//        myservo1.attach(9);
        currentServo = myservo1;
      }
      else if(receivedInt == 2) {
//        myservo2.attach(10);
        currentServo = myservo2;
      }
      else if(receivedInt == 3) {
//        myservo3.attach(11);
        currentServo = myservo3;
      }
      else if(receivedInt == 4) {
//        myservo4.attach(6);
        currentServo = myservo4;
      }
      
      if(receivedInt >= 5 && receivedInt < 180) {
        setServo(receivedInt);
//        delay(1000);
//        currentServo.detach();
      }
//
  }

}

boolean sentAllAngles() {
  if(connectedToPC && Serial.available() == 4) {
    return true;
  }
  else {
    return false;
  }
}

boolean sentFromPC() {
  if(connectedToPC && Serial.available() == 2) {
    return true;
  }
  else {
    return false;
  }
}

boolean userEntered() {
  if(connectedToPC && Serial.available() == 1) {
    return true;
  }
  else {
    return false;
  }
}

void checkHandshake() {
  if(Serial.available() == 5) {
    for(int i = 0; i < 5; i++) {
      inputBytes[i] = Serial.read();
      delay(100);
    }
  }

  if(inputBytes[0] == 16 && inputBytes[1] == 128 && inputBytes[4] == 4) {
    // Hello handshake
    connectedToPC = true;
    Serial.print("HELLO FROM ARDUINO");
    delay(100);
    Serial.print("-READY TO RECEIVE DATA");
    inputBytes[0] = 0;
  }
}

void turnLedOn() {
  digitalWrite(onBoardLed, HIGH);
}

void setServo(int angle) {
  currentServo.write(angle);
}

void turnLedOff() {
  digitalWrite(onBoardLed, LOW);
}

