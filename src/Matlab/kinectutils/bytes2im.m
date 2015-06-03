function [ image ] = bytes2im( bytes )
    bytes = uint8(bytes);
    r = bytes(1:4:end);
    g = bytes(2:4:end);
    b = bytes(3:4:end);
    
    image(:,:,1) = reshape(b,640,480)';
    image(:,:,2) = reshape(g,640,480)';
    image(:,:,3) = reshape(r,640,480)';
end

