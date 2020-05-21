# Curved Animators

## Description
Curve-controlled simple animation solution. It can virtually animate anything you want, and you can easily expand its functionality.

### Key Features
Using AnimationCurve to control the 'progress' has many advantages.
- You can control the animation precisely.
- Easily create looping or pingpong-ing animations.

Also Supports UnityEvent for the inspector use.

## Import

### Dependency
This package is dependent to [NGC6543_Core](https://github.com/niguiecandy/ngc6543_core).

### Unity Package Manifest
1. Open `YourProject/Packages/manifest.json` file.
2. Modify "dependencies" as follows :
	```json
	"dependencies"{
		...
		"com.ngc6543.core": "https://github.com/niguiecandy/ngc6543_core.git",
		"com.ngc6543.curvedanimators": "https://github.com/niguiecandy/curved-animators.git",
		...
	}

## Usage
`CurvedProgressAnimator` class is the base for other classes. It contains the key progress control feature.
The progress curve is clamped to [0, 1] for both time(X-axis) and value(Y-axis). The curve might lie outside the range depending to the tangent settings, which will cause unwanted effects.