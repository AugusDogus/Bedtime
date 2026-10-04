# Artwork

Bedtime follows the forest-green, cream, sage, and brass palette used by
Buildheim, ComfortView, ShieldMeBruhReforged, and Soundheim. The icon and banner
share a bed-and-moon emblem. The SVG files are the editable sources.

The banner is 1200 by 400 pixels; the package icon is 256 by 256 pixels.
Banner lettering uses DejaVu Serif Bold and DejaVu Sans, converted to paths
for consistent rendering. Font notices are in `FONT-LICENSE.txt`.

Render from the repository root with librsvg:

```sh
rsvg-convert assets/artwork/icon.svg -o package/icon.png
rsvg-convert assets/artwork/banner.svg -o package/banner.png
```
