importkinect;
kinect = KinectConnect.Core.Matlab.Kinect(false);

kinect.Start();

h = figure();
axis([-1 1 -1 1])

while(1)
    bytes = kinect.GetImageBytesBgr32640x480;
    if(~isempty(bytes))
        imshow(bytes2im(bytes));
        pause(0.2);
    end
end