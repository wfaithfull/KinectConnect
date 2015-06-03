function scatterface( facepoints )
    p = double(cell2mat(facepoints(:,2)'));
    scatter(p(1,:), p(2,:));
    drawnow;
    pause(0.01);
end

