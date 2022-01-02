Progression Hints Explained


What are Progression Hints?

	Progression Hints are my idea for a hint system that both tries to promote pathing-optimization and giving
	exploration some value, regardless of where the exploration is done. This system even rewards doing harder
	fights if you wish to seek more hint power! (not tested lol)


How do you get a hint?

	Hints are gained every set progression-points obtained (at the time of the first beta, every 7 points
	will reveal a hint).


What is the logic like currently?

	So for the beta version, when 13 hints are given, then any world unhinted world will not contain a proof.


How do you get a progression point?

	Progression Points are rewarded based on some of the major checkpoints within the game and shown on the
	tracker. Reports at this time are considered a check and will give 2 point upon collection. Outside of
	providing a free point, reports serve no purpose and may be removed from the logic here in the future.


Why are point values different?

	Progression Points are weighted based on how difficult it is to go from place to place.
	This means that the difficulty and time is considered for how many points certain progress is rewarded

	For example:
		Going from the beginning of Agrabah to Abu is weighted less than going from Abu to finishing the
		Chasm of Challenges because it essentially requires no real combat/time to go push Abu when doing
		the Chasm requires actual fighting.

		Going from beginning of TWTNW to Roxas is weighted less than Roxas to Xigbar because of the
		readily available quick-run cheese strat you can do on Roxas while Xigbar can be (largely
		considered) a harder fight.


How can I change point values?

	Values are hard-coded into the tracker at the moment, but if you have the source code and can compile
	the tracker yourself, then the values set are in the AutoTracker.cs file. Listed at the bottom of this
	file will be a quick chart of what progress points are valued at.


Why is it 7 points reveals a hint?

	The average points total with 7 as the basis is 233 / 7 = ~33.29, so 33 hints possible. However, this is
	assuming you do all bosses and that's quite the extreme. If all Datas and just roughly harder points are
	removed, then the math is now 131 / 7 = ~18.71. With 13 hints at the moment, this gives some bit of play-room
	with routing so not *everything* has to be done to do get all the hints.
	
	Future idea is to make this limit (7) changeable with a setting in addition to custom point values.
	Another future idea is that obtaining more sets of 7 (or the limit) points tells you where the Proofs are.

	Another idea is that, once proof hinting is set up, that instead of waiting until after 13 hints are revealed
	to reveal the proofs, the amount can be custom set. So 10 regular hints then 3 proofs hinted after.


How can I give direct feedback or report bugs?

	You can feel free to message me directly on discord (codename_geek#2302) for questions or ideas. My
	weighting is definitely not perfect and a complete re-weight may be needed. Any feedback regarding that
	would be appreciated, from newer runners to more experienced/skilled runners.


Current Point Distribution - 

Simulated Twilight Town
	Twilight Thorn		2
	Struggle Fights		2
	Axel			3
	Data Roxas		4

Twilight Town
	Tower			2
	Sandlot			2
	Mickey Mansion		3
	Betwixt			3
	Data Axel		4

Hallow Bastion
	Bailey			2
	Ansem's Study		1
	Corridors		2
	Dancers			2
	Demyx			3
	FF Fights		3
	1k Heartless		4
	Sephiroth		4
	Data Demyx		5

Beast's Castle
	Thresholder		2
	Beast's Room		1
	Dark Thorn		2
	Dragoons		2
	Xaldin			4
	Data Xaldin		5

Olympus Coliseum
	Cerberus		3
	Demyx			1
	Pete			1
	Hydra			3
	Auron Statue		2
	Hades			3
	Zexion			4

Agrabah
	Abu			1
	Chasm of Challenges	2
	Treasure Room		2
	Twin Lords		3
	Carpet 1		3
	Genie Jafar		4
	Lexaeus			4

Land of Dragons
	Cave			3
	Summit			1
	Shan-Yu			2
	Throne Room		3
	Storm Rider		4
	Data Xigbar		5

100 Acre Wood
	Piglet			1
	Rabbit			1
	Kanga			1
	Spooky Cave		3
	Starry Hill		4

Pride Lands
	Simba			1
	Scar			3
	Groundshaker		3
	Data Saix		4

Atlantica
	Tutorial		1
	Ursula			3
	New Day			4

Disney Castle / Timeless River
	Minnie Escort		1
	Old Pete		1
	Windows			3
	Steamboat		1
	Pete			3
	Marluxia		4
	Terra			5

Halloween Town
	Candy Cane Lane		1
	Prison Keeper		3
	Oogie Boogie		2
	Presents		1
	Experiment		4
	Vexen			4

Port Royal
	Town			2
	Barbossa		3
	Gambler			2
	Grim Reaper 2		4
	Data Luxord		5

Space Paranoids
	Screens			3
	Hostile Program		3
	Solar Sailer		3
	MCP			4
	Larxene			4

The World That Never Was
	Roxas			2
	Xigbar			3
	Luxord			2
	Saix			2
	Xemnas			3
	Data Xemnas		5

