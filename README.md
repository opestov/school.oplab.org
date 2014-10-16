# school.oplab.org

Sources of the small static site.

# Build the site

* Install [Node.js](http://www.nodejs.org/) and [npm](https://www.npmjs.org/).
* Mono: *./build.sh*
* Windows: *build.cmd*

To have "dynamic" pages install F# and then edit crontab:

~~~
00 23 * * * cd ~/school.main && /usr/local/bin/fsharpi bricks/group/script-hetzner.fsx && ./build.sh ScheduledUpdate
~~~

# Internals

The short description of used technologies.

## Org mode

Most of the pages are created in Emacs [Org mode](http://orgmode.org/)
and then are exported to html files.

## DotLiquid

The [DotLiquid](https://github.com/formosatek/dotliquid) template
system is used to create actual html pages from raw sources. The first
version used RazorEngine but at that moment there were problems on
Mono with RazorEngine and I switched to the DotLiquid.

## FAKE script

The [FAKE](https://fsharp.github.io/FAKE/) script contains all
instructions required to create site from source parts. For the most
pages it means:

+ get liquid template
+ get html source
+ generate html page
+ copy html page to the *intermediate* directory

The other steps are self-descriptive and can be found in [build.fsx](build.fsx).

## Gulp

I'm using [gulp](http://gulpjs.com/) to perform the following tasks:

+ Concat css files
+ Minify resulting css file
+ Minify html pages

Gulp tasks take data from the *intermediate* directory and put results to the *build* directory.
The gulp tasks are executed from FAKE script.

## Dynamic summary pages

Some pages on this site are updated periodically:

+ The corresponding [bricks/groups](bricks/groups) subdirectory contans all
  required scripts and configuration files.
+ Cron utility is used to run scripts. The script result is set of
  html files in the *dynamic* directory.
+ FAKE script contains target that creates actual pages and copies
  them to the *build* directory.

I'm using F# script that utilizes GroupProgerss library to get
statistics from various online judges and then generates html page
from non-trivial liquid template.
