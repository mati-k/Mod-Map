# Mod Map
 Put province id's over bitmap
 
 All settings are at settings.json
 
 File contains
 
 ```
 {
  "ModFolder": "C:\\Users\\Your\\Documents\\Paradox Interactive\\Europa Universalis IV\\mod\\mod-name", # Path to your mod folder
  "AddMultiSized": true, # Whether to also create maps with increased size
  "MultipliedSize": 2, # Times of size of those maps (Shouldn't have decimals)
  "PrimaryColor": "255, 255, 255", # Basic color of text
  "UseColorContrast": true, # If true, on bright provinces will use secondary darker color
  "ColorContrastThreshold": 500, # Value past which province is considered bright (Simple sum of r+g+b)
  "SecondaryColor": "0, 0, 0", # Secondary color
  "AdditionalBitmaps": [ # Additional bitmaps to also fill with ids, for example if you have some with borders added
    {
      "Path": "ProvincesColoredBlackLines.bmp", # Path to bmp file
      "UseDifferentColor": false, # If set to false, will behave as above
    },
	{
      "Path": "ProvincesOnlyBlackLines.bmp",
      "UseDifferentColor": true, # If set to true will only use unique color (helpful if you have black-white map and such)
      "Color": "220, 100, 100"
    }
  ]
}
 ```
 
 Run ModMap.exe and wait till bitmaps are created
