NET.addAssembly([pwd '/lib/KinectConnect.Core.dll']);
kinect = KinectConnect.Core.Matlab.Kinect(false);

kinect.Start();

axis([-1 1 -1 1])
h = figure();

while(1)
    frame = kinect.GetFaceFrame;
    if(isempty(frame))
        continue;
    end
    [facepoints, projected, animationunits, rotation, translation] = extractFaceData(frame);
    p = double(cell2mat(facepoints(:,2)'));
    scatter3(p(1,:), p(2,:), p(3,:));
    drawnow;
end