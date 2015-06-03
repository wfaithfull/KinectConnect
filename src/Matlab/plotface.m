function plotface(facepoints)

p = double(cell2mat(facepoints(:,2)'));

axis([-1 1 -1 1])
    
hold off
[X,Y] = meshgrid(-0.1:0.008:0.2,0.05:0.008:0.35);
z = griddata(p(:,1),p(:,2),p(:,3),X,Y);
pcolor(X,Y,z)
shading interp
hold on
plot(p(:,1),p(:,2),'k.')
axis tight off
drawnow
    
end