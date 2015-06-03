function [facepoints, projected, animationunits, rotation, translation] = extractFaceData(ff)

    rotation = vec3split(ff.Rotation);
    translation = vec3split(ff.Translation);

    facepoints = dic2mat(ff.FacePoints);
    facepoints(:,2) = cellfun(@(vec) vec3split(vec),facepoints(:,2), 'UniformOutput', 0); 
    projected = dic2mat(ff.ProjectedFacePoints);
    projected(:,2) = cellfun(@(vec) vec2split(vec),projected(:,2), 'UniformOutput', 0); 
    animationunits = dic2mat(ff.AnimationUnits);
    
end