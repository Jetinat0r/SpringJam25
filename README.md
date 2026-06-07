# SpringJam25
 
This branch is for Evan to test fixes to things that bother him, pending possible future addition into the main game.

## Credits Improvements
- Fixed flickering I's in special credits
- Added more space between the wall and the names sandwiching it
- Moved Tess above Marcie (my sisters) so they're together, and moved Tony Li in Tess's former place
- Swapped out the spotlight for a candle in the Shady credits room
- Made shadow Shady walk left and then look down in the Shady credits room

## Minor/Miscellaneous Tweaks
- Added missing unshadow effects for level clearing as a shadow (this has always bothered me)
- Fixed no sound feedback for musical machines toggling objects
- Adjusted vertical keyboard/controller navigation behavior for inactive tabs in the settings menu
  - The back button goes down to the most recently selected tab, and the first navigable element goes up to the most recently selected tab. (Note selected != active)
- Fixed Shady getting nudged by robo-vacs while resetting or winning (because they are immune in this state, and it looks dumb particularly during resets + can cause clipping outside the wall)
  - Instead the robo-vacs ignore collision with Shady and flip if they are within 0.5 units of Shady in these states

## Controversial Player Changes
- Fixed "can shadow" checks not ensuring light and wall actually intersect
- Standardized wall/light detector hitboxes in an attempt to fix instant unshadowing. Caveats:
  - Leftover downward velocity can still push you out of the can shadow zone
  - Now the shadow/light detector hitboxes miss most of Shady's head, which might have adverse consequences that need testing
- Added input buffering and hold options for the shadow control while in ghost mode (to address MicalPixel's feedback)
  - Can independently control, via inspector:
    - How long the shadow input can be buffered for (0 = disabled)
      - Included extra safeguards for holding down shadow longer than this, since I was getting instantly unshadowed. There is a lock placed on re-shadowing via the input buffer until you've released the input
    - Whether shadow can be preemptively held before touching a lit wall in order to turn you into a shadow once you touch a lit wall, for one time ever while the control is held
    - Whether shadow can be persistently held in order to turn into a shadow whenever you touch a lit wall as a ghost
      - The persistent hold has some obvious problems when holding the shadow control and crawling up - you can get into a loop where you are force unshadowed, fall down back into the light, reshadow, crawl up, rinse and repeat
      - It's also not preferable for accessibility according to MicalPixel, but I figured I'd at the very least try implementing it as an option
  - The most accessible combination is 0.2 input buffer time and allowing preemptively held shadow, which is what MicalPixel also landed on. We can do some testing with this at a later time, and it may be worth including an accessibility settings panel in settings later down the line to configure things like this
