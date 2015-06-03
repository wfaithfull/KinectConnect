NET.addAssembly([pwd '/lib/KinectConnect.Core.dll']);
kinect = KinectConnect.Core.Matlab.Kinect(false);

kinect.Start();

axis([-1 1 -1 1])
h = figure();

while(1)
    image = kinect.GetColorImage;
    if(~isempty(image))
        imshow(kinect.GetColorImage);
    end
end