# Husky Robotics Simulator
The 3D Unity-based Husky Robotics simulator.

## Overview
This simulator creates a WebSocket client to connect directly to the rover WebSocket server at the URL `ws://localhost:3001/simulator`. The simulator will automatically connect to the rover server and reconnect as needed, providing a visual indication of the connection status. The simulator and rover server communicate with each other by sending JSON objects termed *messages* over the WebSocket connection. Through these messages, the rover server can request that the simulator perform certain actions, such as set a a motor's power. Additionally, the simulator provides the rover server with data such as camera streams and lidar data through these messages.

## Using the Simulator
1. Download the latest release for your operating system.
2. Run the executable included with your downloaded release.
3. Start the [rover server](https://github.com/huskyroboticsteam/Resurgence). Optionally start [Mission Control](https://github.com/huskyroboticsteam/new-mission-control) if you would like to teleoperate the rover yourself. The simulator will automatically connect to the rover server and begin sending and receiving messages.

## Motors
The simulator is able to simulate the motors with the following names:
- frontLeftWheel
- frontRightWheel
- rearLeftWheel
- rearRightWheel
- armBase
- shoulder
- elbow

## Cameras
The simulator is able to simulate the cameras with the following names:
- front
- rear
- upperArm

## Messages
The JSON objects sent between the simulator and the rover server are termed *messages*. Each message has a type property and a number of additional parameters depending on the type. Each type is prefaced with "sim" to avoid confusion with messages pertaining to Mission Control. The usage of each type of message is detailed below.

## Motor Power Request
### Description
Sent from the rover server to instruct the simulator to make a motor run to a position.

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
Sent from the rover server to instruct the simulator to make a motor run to a position.

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
- `position` - the requested position in degrees

## Motor Velocity Request
### Description
Sent from the rover server to instruct the simulator to make a motor run with a velocity.

### Syntax
```
{
  type: "simMotorVelocityRequest",
  motor: string,
  velocity: number
}
```

### Parameters
- `motor` - the name of the motor
- `velocity` - the requested velocity in degrees per second

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
  height: number
}
```

### Parameters
- `camera` - the name of the camera
- `fps` - the frames per second of the stream
- `width` - the width of the stream
- `height` - the height of the stream

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

## Motor Status Report
### Description
Sent from the simulator to inform the rover server of a motor's status.

### Syntax
```
{
  type: "simMotorStatusReport",
  motor: string,
  power: number | null,
  position: number | null,
  velocity: number | null
}
```

### Parameters
- `motor` - the name of the motor
- `power` - the current power of the motor, or null if unavailable
- `position` - the current position of the motor, or null if unavailable
- `velocity` - the current velocity of the motor, or null if unavailable

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
