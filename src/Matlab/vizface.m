importkinect;
kinect = KinectConnect.Core.Matlab.Kinect(false);

kinect.Start();

h = figure();
axis([-1 1 -1 1])

while(1)
    frame = kinect.GetFaceFrame;
    pause(0.2);
    if(~isempty(frame))
        [facepoints, projected, animationunits, rotation, translation] = extractFaceData(frame);
        p = double(cell2mat(facepoints(:,2)'));
        scatter3(p(1,:), p(2,:), p(3,:));
        drawnow;
    end
end