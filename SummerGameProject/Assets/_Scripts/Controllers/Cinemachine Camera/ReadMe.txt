Daniel Fiuk

Remember to add an empty game object as a child to the player object.
Add the Follow Target script to it.
In the "CM vcam1" object's inspector, set the Follow Target to the aforementioned game object.

If you wish to transition to Unity's new Input System, Add the input provider extention to the CM vcam1 object,
and set the input to your respective look control. 

For mouse, use "Mouse Delta".
For gamepad, use "Right Stick".