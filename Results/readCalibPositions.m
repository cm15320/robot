function readCalibPositions
    A = csvread('calibrationPositions.csv');
    B = csvread('testPositions.csv');

    
    x = A(:, 1);
    z = A(:, 2);
    y = A(:, 3);
    
    x1 = B(:, 1);
    x3 = B(:, 2);
    x2 = B(:, 3);
    
    figure('color','white'); 
    scatter3(x, y, z, 'Marker', '.');
    hold on
    scatter3(x1, x2, x3, 30, 'r', 'filled');
    hold on
    legend('Calibration Point', 'Test Point');
    
    
    
    tri = delaunay(A(:, 1), A(:,2));
    
   
%     line(x,y,z);
% 
 %Create and apply the colormap

c = zeros(64,3);

for i = 1:64

      c(i,1) = (i+32)/100;

      c(i,2) = i/100;

      c(i,3) = i/100;

end

colormap(c);

%Plot the surface
  figure('color','white'); 
shading interp
hObj = trisurf(tri,x,y,z,'FaceColor','interp','FaceLighting','phong');
% 
% figure('color','white');
%    c = gradient(z);
%    k = hypot(x,y)<3;
%    plot3k({x(k) y(k) z(k)}, ...
%       'Plottype','stem','FontSize',12, ...
%       'ColorData',c(k),'ColorRange',[-0.5 0.5],'Marker',{'o',2}, ...
%       'Labels',{'Peaks','Radius','','Intensity','Lux'});

    
%     [X,Y, Z] = meshgrid(x, y, z);  % Create a regular grid
%     F = scatteredInterpolant(x,y,z);       % Create an interpolant
%     Z = F(X,Y);   % Evaluate the interpolant at the
%     mesh(X,Y,Z);  % Plot the interpolated mesh
%     surf(x, y, z);
%     

%     dx=0.001;
%     dy=0.001;
% 
%     x_edge=[floor(min(x)):dx:ceil(max(x))];
%     y_edge=[floor(min(y)):dy:ceil(max(y))];
%     [X,Y]=meshgrid(x_edge,y_edge);
%     Z=griddata(x,y,z,X,Y);
%     
%     mesh(X,Y,Z)
end