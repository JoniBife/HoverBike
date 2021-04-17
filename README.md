
## HoverBike

A unity project that contains the **controller and camera scripts for a simple hover bike**, inspired from Destiny II sparrows. 
The controller exposes a number of settings that can be viewed from the unity editor:

 - Hover height
 - Hover force
 - Dampening (controls the hover bounciness, more dampening less bounciness)
 
A hover bike has a set of rover engines and from each a raycast is sent to the -local up direction, depending on the hit distance a different force is calculated and applied to maintain the hovering. I also used Hooke's Law to calculate a dampen force to reduce the bounciness.
