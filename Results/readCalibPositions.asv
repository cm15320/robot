function readCalibPositions
    A = csvread('calibrationPositions.csv');
%     scatter3(A(:,1), A(:,2), A(:,3), 'Marker', '.');
% %     tri = delaunay(A(:, 1), A(:,2));
% 
    x = A(:, 1);
    y = A(:, 2);
    z = A(:, 3);
    
    [X,Y, Z] = meshgrid(x, y, z);  % Create a regular grid
    F = scatteredInterpolant(x,y,z);       % Create an interpolant
    Z = F(X,Y);   % Evaluate the interpolant at the
    mesh(X,Y,Z);  % Plot the interpolated mesh
    surf(x, y, z);
    
end