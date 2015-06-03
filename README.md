# KinectConnect
Middleware to serve kinect data to other applications

## Want to help?
* Tests
* Tests!!
* SDK 2x support

### Limitations to be aware of
* x64 only
* Windows only
* Currently Kinect SDK 1.8 only.

### Features
| Feature       | Namespace     |
| ------------- | ------------- |
| Kinect handle designed for use in Matlab  | KinectConnect.Core.Matlab |
| Socket server delivering JSON serialized face points | KinectConnect.Server |

### Matlab
Example usage in matlab, including .m files for conversion and assistance can be found in the ```src/Matlab``` directory.

1. Build the project in Release configuration.
2. A post-build event will copy the binaries into ```src/Matlab/lib```
3. Test the examples
