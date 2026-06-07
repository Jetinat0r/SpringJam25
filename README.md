# EvanFixes
 
This branch is for me (Evan) to test fixes to things that bother me, pending possible future addition into the main game.

## Credits Improvements
- Fixed flickering I's in special credits
- Added more space between the wall and the names sandwiching it
- Moved Tess above Marcie (my sisters) so they're together, and moved Tony Li in Tess's former place
- Swapped out the spotlight for a candle in the Shady credits room
- Made shadow Shady walk left and then look down in the Shady credits room

## Minor/Miscellaneous Tweaks
- Added missing unshadow effects for level clearing as a shadow (this has always bothered me)
- Fixed no sound feedback for musical machines toggling objects
- Fixed Shady getting nudged by robo-vacs while dying, resetting, or winning (because Shady is immune in these states, and it looks dumb particularly during resets + can cause clipping outside the wall)
  - Instead the robo-vacs ignore collision with Shady, and flip if they are within 0.5 units of Shady (unless he is dying)

## Controversial Changes
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
- Adjusted vertical keyboard/controller navigation behavior for inactive tabs in the settings menu
  - The back button goes down to the most recently selected tab, and the first navigable element goes up to the most recently selected tab. (Note selected != active)
  - The prior behavior was intentional by Jet: "oh right, the settings tabs always bringing you back to the active tab was intentional. right idea? who knows, but it makes sense and will always bring you back to the right place"
    - Also Jet: "on the other side of the coin you could misinput while trying to go down, then 20 seconds later you nav back to the bar and end up not on the tab you think you should be in. no real winning here"

-----

## Possible Future Targets
- Add accessibility tab to configure the input buffering/holding behavior of the shadow control
- Fix menu play button sending you to your highest level (aka always 32 if you beat the game), not your most recently played level
- Fix menu play button nuking challenge preferences
  - Potentially store challenge preferences between game sessions if that's not already done
- Fix toggling shadow at cam zone boundary triggering it
  - Connected to (if not directly caused by) instant unshadow fix
- Fix toggling shadow facing Shady a different way than ghost Shady, whichever one shadow Shady had been facing most recently
  - Jet: "rotating shadow shady & flipping ghost shady isn't impossible to do, but also people are usually moving so they don't even notice"
- Fix level select vertical navigation from world stepper buttons and challenge buttons not remembering the most recently selected level
  - Jet thinks this opens up a pandora's box of problems and it is probably way too insignificant to invest time into, which is fair enough
    - "i completely understand (and the same would be done to the bottom challenge buttons) but it would have to also properly account for what happens when you change worlds, and deal with the mouse trying to fight all of these systems simultaneously"
    - "what happens if we nav to the bottom row and mouse onto the world change buttons? we don't want to go back to the bottom row, etc. just tiny little things of patches upon patches when our game already works"
- Fix opening new settings tab with keyboard/controller navigation while cursor is hovering an element focusing that element even if the cursor has not moved recently
  - Jet: "that mouse thing is whatever, people don't usually do keyboard nav while using the mouse, and if you're doing keyboard nav, you shove your mouse offscreen bc it gets in the way (it's also really hard to solve based on how our nav works!)"
