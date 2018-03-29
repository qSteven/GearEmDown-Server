# Gear 'em Down - Unity Server
A authoritative tower defense game server. Works in conjunction with client browsers and websocket.
The entire game logic runs here. Every client is only running a visual representation of the game.

![Unity editor view](http://qwellcode.de/github/GearEmDown_UnityView.png)

## Basic requirements
- Unity 2017.3.1f1

## Clients
You can find the A-Frame version of the client here: [Link zum Repo wenn es öffentlich ist]
A Decentraland version is planed but not determent.

## Building a server version
### Build
1. Open up the project in unity. Go to `File` -> `Build Settings...`.
2. Choose your target platform and architecture. Check the "Headless Mode" option, if you are compiling for Linux.
3. Press `Build`, choose a name and your target directory.
4. Move the executable, "_Data" folder and any additional files (if any) to your desired directory on your server.
5. Navigate inside the directory with your command line editor.
6. Run the executable with the following parameters `-batchmode -nographics -port 8080`. You can replace `8080` with another port if you need.

Example (Linux):
```
./GearEmDown_Server.x86_64 -batchmode -nographics -port 8080
```

### Download
You can find an already compiled Linux version here: http://qwellcode.com/github/GearEmDown_Server_v1.0.zip

## Additional Notes
This unity project was developed by interns as a game development project for education purposes. All the 3D models, shader and a rudimentary UI is implemented in this project for testing and later use. If you plan on hosting a server, make sure to build a "headless" version and run it with `-batchmode -nographics` in the commandline.

## License
This project is licensed under the MIT License - see the \[LICENSE\](https://github.com/qSteven/GearEmDown-Server/blob/master/LICENSE) file for details.
