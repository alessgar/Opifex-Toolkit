# Opifex Toolkit
 A program designed to speed up the process of long Garry's Mod development tasks.

## Features
 - Content Pack Generator
   - Can pull resources from any supported source engine game to create a content pack that removes the requirement for owning the original games. (Useful for maps that use CSS content, HL2EP2 content, etc.)
   - Currently supports ``Half Life 2``, ``Black Mesa``, ``INFRA``, ``Counter Strike: Source``, ``Half Life: Source``, ``Half Life 2: Episode 1``, ``Half Life 2: Episode 2``, ``Counter Strike: Global Offensive``, ``Portal``, ``Portal 2``, ``Left 4 Dead``, ``Left 4 Dead 2``, ``Team Fortress 2``
   - Due to the lack of any sounds within the VPKs, sounds are unable to be imported automatically from L4D and L4D2. 
   - Games must be physically installed through Steam
   - If you want another game supported, create a ticket through github
   - LOD Stripper option for ported models
   - Ability to skip map recreation for creation of prop packs
   
 - Content Pack Shrinker
   - Can compare a given VMF's required resources to a given content pack folder in order to create a separate content pack consisting of only what is needed. (Useful for post-development cleanup or finding specific files within your game's materials/models/sounds folders.)
  
 - SteamUGC Based Workshop Uploader
   - Coming Soon

## Dependencies:
 - Requires .NET Framework v4.7.2 or higher
 - .NET Framework can be found at https://dotnet.microsoft.com/download

## Support:
  - If you need support, please feel free to submit a github ticket, although I do not actively develop this program anymore as I have moved past gmod development for the most part.

## Feature Additions
Have a new feature that you think is useful enough to make its place in The Opifex Toolkit? Please make a Pull Request with your tool implemented into the current source code and I will review it as soon as possible.
  
## License
The Opifex Toolkit uses the GNU General Public License v3.0 and can be viewed in the file ``LICENSE``
