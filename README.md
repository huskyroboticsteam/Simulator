# Husky Robotics Simulator
The 3D Unity-based Husky Robotics simulator.

## Overview
This simulator creates a WebSocket client to connect directly to the rover WebSocket server at the URL `ws://localhost:3001/simulator`. The simulator will automatically connect to the rover server and reconnect as needed, providing a visual indication of the connection status. The simulator and the rover server communicate with each other by sending JSON objects termed *messages* over the WebSocket connection. Through these messages, the rover server can request that the simulator perform certain actions, such as set a a motor's power. Additionally, the simulator provides the rover server with data such as camera streams and lidar data through these messages.

## Using the Simulator
1. Download the latest release for your operating system.
2. Give the executable included with your download permission to run as an executable.
3. Run the executable.
4. Start the [rover server](https://github.com/huskyroboticsteam/Resurgence). Optionally, start [Mission Control](https://github.com/huskyroboticsteam/new-mission-control) if you would like to teleoperate the rover yourself. The simulator will automatically connect to the rover server and begin sending and receiving messages.

## Motors
The simulator is able to simulate the motors with the following names:
- frontLeftWheel
- frontRightWheel
- rearLeftWheel
- rearRightWheel
- armBase
- shoulder
- elbow
- forearm
- differentialLeft
- differentialRight
- hand

## Cameras
The simulator is able to simulate the cameras with the following names:
- front
- rear
- upperArm

## Additional Hardware Devices
The simulator is also able to simulate the following hardware devices:
- GPS sensor
- IMU
- Lidar sensor

## Messages
The JSON objects sent between the simulator and the rover server are termed *messages*. Each message has a type property and a number of additional parameters depending on the type. Each type is prefaced with "sim" to avoid confusion with messages pertaining to Mission Control. The usage of each type of message is detailed below.

## Motor Power Request
### Description
Sent from the rover server to instruct the simulator to make a motor run with a specified power.

### Syntax
```
{
  type: "simMotorPowerRequest",
  motor: string,
  power: number
}
```

### Parameters
- `motor` - the name of the motor
- `power` - the requested power in [-1, 1]

## Motor Position Request
### Description
Sent from the rover server to instruct the simulator to make a motor run to a specified position.

### Syntax
```
{
  type: "simMotorPositionRequest",
  motor: string,
  position: number
}
```

### Parameters
- `motor` - the name of the motor
- `position` - the requested position in millidegrees

## Motor Status Report
### Description
Sent from the simulator to inform the rover server of a motor's status.

### Syntax
```
{
  type: "simMotorStatusReport",
  motor: string,
  power: number | null,
  position: number | null
}
```

### Parameters
- `motor` - the name of the motor
- `power` - the current power of the motor in [-1, 1], or `null` if unavailable
- `position` - the current integer position of the motor in millidegrees, or `null` if unavailable

## Motor Limit Switch Alert
### Description
Sent from the simulator to inform the rover server that a motor has triggered its limit switch.

### Syntax
```
{
  type: "simLimitSwitchAlert",
  motor: string,
  limit: "minimum" | "maximum"
}
```

### Parameters
- `motor` - the name of the motor

## Camera Stream Open Request
### Description
Sent from the rover server to instruct the simulator to begin providing a camera stream.

### Syntax
```
{
  type: "simCameraStreamOpenRequest",
  camera: string,
  fps: number,
  width: number,
  height: number,
  intrinsicParameters: number[9] | null
}
```

### Parameters
- `camera` - the name of the camera
- `fps` - the frames per second of the stream
- `width` - the width of the stream in pixels
- `height` - the height of the stream in pixels
- `intrinsicParameters` - the intrinsic parameters to be used by the camera, or `null` if none should be specified

## Camera Stream Close Request
### Description
Sent from the rover server to instruct the simulator to stop providing a camera stream.

### Syntax
```
{
  type: "simCameraStreamCloseRequest",
  camera: string
}
```

### Parameters
- `camera` - the name of the camera

## Camera Stream Report
### Description
Sent from the simulator to inform the rover server of a single frame of a camera stream.

### Syntax
```
{
  type: "simCameraStreamReport",
  camera: string,
  data: string
}
```

### Parameters
- `camera` - the name of the camera
- `data` - the frame in JPG format encoded as a base-64 string

## Rover True Pose Report
### Description
Sent from the simulator to inform the rover server of the rover's exact pose.

### Syntax
```
{
  type: "simRoverTruePoseReport",
  position: {
    x: number,
    y: number,
    z: number
  },
  rotation: {
    x: number,
    y: number,
    z: number,
    w: number
  }
}
```

### Parameters
- `position` - the position of the rover in standard Husky Robotics coordinates
- `rotation` - the rotation of the rover in standard Husky Robotics coordinates

## GPS Position Report
### Description
Sent from the simulator to inform the rover server of the geographic position provided by a simulated GPS sensor. The position will be reported in standard geographic coordinates. The simulated GPS sensor will map Unity's cartesian origin to Null Island. Gaussian noise is applied to the latitude and longitude.

### Syntax
```
{
  type: "simGpsPositionReport",
  latitude: number,
  longitude: number
}
```

### Parameters
- `latitude` - the latitude in degrees
- `longitude` - the longitude in degrees

## IMU Orientation Report
### Description
Sent from the simulator to inform the rover server of the orientation provided by a simulated IMU. The orientation will be a quaternion in the standard Husky Robotics software coordinate system.

### Syntax
```
{
  type: "simImuOrientationReport",
  x: number,
  y: number,
  z: number,
  w: number
}
```

### Parameters
- `x` - The x-component of the orientation
- `y` - The y-component of the orientation
- `z` - The z-component of the orientation
- `w` - The w-component of the orientation

## Lidar Data Report
### Description
Sent from the simulator to inform the rover server of data provided by a simulated lidar sensor.

### Syntax
```
{
  type: "simLidarReport",
  points: { r: number, theta: number }[]
}
```

### Parameters
- `points` - an array of points in polar coordinates read by the simulated lidar sensor
- `r` - the distance between a point and the rover in meters
- `theta` - the angle of a point in [0, 2π) measured from the rover's forward direction and increasing counterclockwise
