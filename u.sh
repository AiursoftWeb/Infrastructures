#!/bin/bash
#
# Script migración desde Fontawesome v4 a Fontawesome v5
#
# Modo de uso
#  ./fontawesome4to5.sh [-d DIRECTORIO] [-e EXTENSION] [-v]
#
# Si no se especifica DIRECTORIO, se hará el reemplazo en el directorio actual
# Si no se especifica EXTENSION, se buscarán por defecto archivos "php"
# Opción -v mostrará los archivos que se van modificando
#
# PD: respalda tu proyecto ANTES de usar este script :-)
#
# Instrucciones migración: https://fontawesome.com/how-to-use/upgrading-from-4
#
# @author Esteban De La Fuente Rubio, DeLaF (esteban[at]delaf.cl)
# @version 2017-12-26
#

# iconos que deben ser corregidos, todos con prefijo 'fa' en v4
ICONS=(
    # v4 nombre             # v5 nombre                                 # v5 prefijo
    address-book-o          address-book                                far
    address-card-o          address-card                                far
    adn                     app-store-ios                               fab
    area-chart              chart-area                                  fas
    arrow-circle-o-down     arrow-alt-circle-down                       far
    arrow-circle-o-left     arrow-alt-circle-left                       far
    arrow-circle-o-right    arrow-alt-circle-right                      far
    arrow-circle-o-up       arrow-alt-circle-up                         far
    arrows-alt              expand-arrows-alt                           fas
    arrows-h                arrows-alt-h                                fas
    arrows-v                arrows-alt-v                                fas
    arrows                  arrows-alt                                  fas
    asl-interpreting        american-sign-language-interpreting         fas
    automobile              car                                         fas
    bank                    university                                  fas
    bar-chart-o             chart-bar                                   far
    bar-chart               chart-bar                                   far
    bathtub                 bath                                        fas
    battery-0               battery-empty                               fas
    battery-1               battery-quarter                             fas
    battery-2               battery-half                                fas
    battery-3               battery-three-quarters                      fas
    battery-4               battery-full                                fas
    battery                 battery-full                                fas
    bell-o                  bell                                        far
    bell-slash-o            bell-slash                                  far
    bitbucket-square        bitbucket                                   fab
    bitcoin                 btc                                         fab
    bookmark-o              bookmark                                    far
    building-o              building                                    far
    cab                     taxi                                        fas
    calendar-check-o        calendar-check                              far
    calendar-minus-o        calendar-minus                              far
    calendar-o              calendar                                    far
    calendar-plus-o         calendar-plus                               far
    calendar-times-o        calendar-times                              far
    calendar                calendar-alt                                fas
    caret-square-o-down     caret-square-down                           far
    caret-square-o-left     caret-square-left                           far
    caret-square-o-right    caret-square-right                          far
    caret-square-o-up       caret-square-up                             far
    cc                      closed-captioning                           far
    chain-broken            unlink                                      fas
    chain                   link                                        fas
    check-circle-o          check-circle                                far
    check-square-o          check-square                                far
    circle-o-notch          circle-notch                                fas
    circle-o                circle                                      far
    circle-thin             circle                                      far
    clock-o                 clock                                       far
    close                   times                                       fas
    cloud-download          cloud-download-alt                          fas
    cloud-upload            cloud-upload-alt                            fas
    cny                     yen-sign                                    fas
    code-fork               code-branch                                 fas
    comment-o               comment                                     far
    commenting-o            comment-alt                                 far
    commenting              comment-alt                                 fas
    comments-o              comments                                    far
    credit-card-alt         credit-card                                 fas
    cutlery                 utensils                                    fas
    dashboard               tachometer-alt                              fas
    deafness                deaf                                        fas
    dedent                  outdent                                     fas
    diamond                 gem                                         far
    dollar                  dollar-sign                                 fas
    dot-circle-o            dot-circle                                  far
    drivers-license-o       id-card                                     far
    drivers-license         id-card                                     fas
    eercast                 sellcast                                    fab
    envelope-o              envelope                                    far
    envelope-open-o         envelope-open                               far
    eur                     euro-sign                                   fas
    euro                    euro-sign                                   fas
    exchange                exchange-alt                                fas
    external-link-square    external-link-square-alt                    fas
    external-link           external-link-alt                           fas
    eyedropper              eye-dropper                                 fas
    fa                      font-awesome                                fab
    facebook-f              facebook-f                                  fab
    facebook-official       facebook                                    fab
    facebook                facebook-f                                  fab
    feed                    rss                                         fas
    file-archive-o          file-archive                                far
    file-audio-o            file-audio                                  far
    file-code-o             file-code                                   far
    file-excel-o            file-excel                                  far
    file-image-o            file-image                                  far
    file-movie-o            file-video                                  far
    file-o                  file                                        far
    file-pdf-o              file-pdf                                    far
    file-photo-o            file-image                                  far
    file-picture-o          file-image                                  far
    file-powerpoint-o       file-powerpoint                             far
    file-sound-o            file-audio                                  far
    file-text-o             file-alt                                    far
    file-text               file-alt                                    fas
    file-video-o            file-video                                  far
    file-word-o             file-word                                   far
    file-zip-o              file-archive                                far
    files-o                 copy                                        far
    flag-o                  flag                                        far
    flash                   bolt                                        fas
    floppy-o                save                                        far
    folder-o                folder                                      far
    folder-open-o           folder-open                                 far
    frown-o                 frown                                       far
    futbol-o                futbol                                      far
    gbp                     pound-sign                                  fas
    ge                      empire                                      fab
    gear                    cog                                         fas
    gears                   cogs                                        fas
    gittip                  gratipay                                    fab
    github                  github                                    fab
    glass                   glass-martini                               fas
    google-plus-circle      google-plus                                 fab
    google-plus-official    google-plus                                 fab
    google-plus             google-plus-g                               fab
    group                   users                                       fas
    hand-grab-o             hand-rock                                   far
    hand-lizard-o           hand-lizard                                 far
    hand-o-down             hand-point-down                             far
    hand-o-left             hand-point-left                             far
    hand-o-right            hand-point-right                            far
    hand-o-up               hand-point-up                               far
    hand-paper-o            hand-paper                                  far
    hand-peace-o            hand-peace                                  far
    hand-pointer-o          hand-pointer                                far
    hand-rock-o             hand-rock                                   far
    hand-scissors-o         hand-scissors                               far
    hand-spock-o            hand-spock                                  far
    hand-stop-o             hand-paper                                  far
    handshake-o             handshake                                   far
    hard-of-hearing         deaf                                        fas
    hdd-o                   hdd                                         far
    header                  heading                                     fas
    heart-o                 heart                                       far
    hospital-o              hospital                                    far
    hotel                   bed                                         fas
    hourglass-1             hourglass-start                             fas
    hourglass-2             hourglass-half                              fas
    hourglass-3             hourglass-end                               fas
    hourglass-o             hourglass                                   far
    id-card-o               id-card                                     far
    ils                     shekel-sign                                 fas
    image                   image                                       far
    inr                     rupee-sign                                  fas
    institution             university                                  fas
    intersex                transgender                                 fas
    jpy                     yen-sign                                    fas
    keyboard-o              keyboard                                    far
    krw                     won-sign                                    fas
    legal                   gavel                                       fas
    lemon-o                 lemon                                       far
    level-down              level-down-alt                              fas
    level-up                level-up-alt                                fas
    life-bouy               life-ring                                   far
    life-buoy               life-ring                                   far
    life-saver              life-ring                                   far
    lightbulb-o             lightbulb                                   far
    line-chart              chart-line                                  fas
    linkedin-square         linkedin                                    fab
    linkedin                linkedin-in                                 fab
    long-arrow-down         long-arrow-alt-down                         fas
    long-arrow-left         long-arrow-alt-left                         fas
    long-arrow-right        long-arrow-alt-right                        fas
    long-arrow-up           long-arrow-alt-up                           fas
    mail-forward            share                                       fas
    mail-reply-all          reply-all                                   fas
    mail-reply              reply                                       fas
    map-marker              map-marker-alt                              fas
    map-o                   map                                         far
    meanpath                font-awesome                                fab
    meh-o                   meh                                         far
    minus-square-o          minus-square                                far
    mobile-phone            mobile-alt                                  fas
    mobile                  mobile-alt                                  fas
    money                   money-bill-alt                              far
    moon-o                  moon                                        far
    mortar-board            graduation-cap                              fas
    navicon                 bars                                        fas
    newspaper-o             newspaper                                   far
    paper-plane-o           paper-plane                                 far
    paste                   clipboard                                   far
    pause-circle-o          pause-circle                                far
    pencil-square-o         edit                                        far
    pencil-square           pen-square                                  fas
    pencil                  pencil-alt                                  fas
    photo                   image                                       far
    picture-o               image                                       far
    pie-chart               chart-pie                                   fas
    play-circle-o           play-circle                                 far
    plus-square-o           plus-square                                 far
    question-circle-o       question-circle                             far
    ra                      rebel                                       fab
    refresh                 sync                                        fas
    remove                  times                                       fas
    reorder                 bars                                        fas
    repeat                  redo                                        fas
    resistance              rebel                                       fab
    rmb                     yen-sign                                    fas
    rotate-left             undo                                        fas
    rotate-right            redo                                        fas
    rouble                  ruble-sign                                  fas
    rub                     ruble-sign                                  fas
    ruble                   ruble-sign                                  fas
    rupee                   rupee-sign                                  fas
    s15                     bath                                        fas
    scissors                cut                                         fas
    send-o                  paper-plane                                 far
    send                    paper-plane                                 fas
    share-square-o          share-square                                far
    shekel                  shekel-sign                                 fas
    sheqel                  shekel-sign                                 fas
    shield                  shield-alt                                  fas
    sign-in                 sign-in-alt                                 fas
    sign-out                sign-out-alt                                fas
    signing                 sign-language                               fas
    sliders                 sliders-h                                   fas
    smile-o                 smile                                       far
    snowflake-o             snowflake                                   far
    soccer-ball-o           futbol                                      far
    sort-alpha-asc          sort-alpha-down                             fas
    sort-alpha-desc         sort-alpha-up                               fas
    sort-amount-asc         sort-amount-down                            fas
    sort-amount-desc        sort-amount-up                              fas
    sort-asc                sort-up                                     fas
    sort-desc               sort-down                                   fas
    sort-numeric-asc        sort-numeric-down                           fas
    sort-numeric-desc       sort-numeric-up                             fas
    spoon                   utensil-spoon                               fas
    square-o                square                                      far
    star-half-empty         star-half                                   far
    star-half-full          star-half                                   far
    star-half-o             star-half                                   far
    star-o                  star                                        far
    sticky-note-o           sticky-note                                 far
    stop-circle-o           stop-circle                                 far
    sun-o                   sun                                         far
    support                 life-ring                                   far
    tablet                  tablet-alt                                  fas
    tachometer              tachometer-alt                              fas
    television              tv                                          fas
    thermometer-0           thermometer-empty                           fas
    thermometer-1           thermometer-quarter                         fas
    thermometer-2           thermometer-half                            fas
    thermometer-3           thermometer-three-quarters                  fas
    thermometer-4           thermometer-full                            fas
    thermometer             thermometer-full                            fas
    thumb-tack              thumbtack                                   fas
    thumbs-o-down           thumbs-down                                 far
    thumbs-o-up             thumbs-up                                   far
    ticket                  ticket-alt                                  fas
    times-circle-o          times-circle                                far
    times-rectangle-o       window-close                                far
    times-rectangle         window-close                                fas
    toggle-down             caret-square-down                           far
    toggle-left             caret-square-left                           far
    toggle-right            caret-square-right                          far
    toggle-up               caret-square-up                             far
    trash-o                 trash-alt                                   far
    trash                   trash-alt                                   fas
    try                     lira-sign                                   fas
    turkish-lira            lira-sign                                   fas
    unsorted                sort                                        fas
    usd                     dollar-sign                                 fas
    user-circle-o           user-circle                                 far
    user-o                  user                                        far
    vcard-o                 address-card                                far
    vcard                   address-card                                fas
    video-camera            video                                       fas
    vimeo                   vimeo-v                                     fab
    volume-control-phone    phone-volume                                fas
    warning                 exclamation-triangle                        fas
    wechat                  weixin                                      fab
    wheelchair-alt          accessible-icon                             fab
    window-close-o          window-close                                far
    won                     won-sign                                    fas
    y-combinator-square     hacker-news                                 fab
    yc-square               hacker-news                                 fab
    yc                      y-combinator                                fab
    yen                     yen-sign                                    fas
    youtube-play            youtube                                     fab
    youtube-square          youtube                                     fab
    # otros iconos agregados que no estaban en el listado de migración oficial
    paypal                  paypal                                      fab
    telegram                telegram                                    fab
    dropbox                 dropbox                                     fab
    wordpress               wordpress                                   fab
    rebel                   rebel                                       fab
)

# opciones por defecto
DIR="."
EXT="*.cshtml"
VERBOSE=0

# argumentos del script
while getopts d:e:v option
do
    case "${option}"
    in
        d) DIR=${OPTARG};;
        e) EXT="*.${OPTARG}";;
        v) VERBOSE=1;;
    esac
done

# regla de filtrado
REGLA_FILTRADO=""
REGLA_REEMPLAZO=""
i=0
while [ $i -lt ${#ICONS[@]} ]
do
    V4_ICON=${ICONS[$i]}
    V5_ICON=${ICONS[$i+1]}
    V5_PREFIX=${ICONS[$i+2]}
    REGLA_FILTRADO="$REGLA_FILTRADO -e fa[[:blank:]]fa-$V4_ICON"
    REGLA_REEMPLAZO="$REGLA_REEMPLAZO -e s/fa[[:space:]]fa-${V4_ICON}/${V5_PREFIX}{{{SPACE}}}fa-${V5_ICON}/gI"
    i=`expr $i + 3`
done

# buscar archivos que contienen class v4 que se debe reemplazar
ARCHIVOS=`grep -i $REGLA_FILTRADO --include=$EXT -r $DIR | awk -F ':' '{print $1}' | sort -u`

# realizar reemplazo en cada archivo encontrado
for ARCHIVO in $ARCHIVOS
do
    [ $VERBOSE -eq 1 ] && echo $ARCHIVO
    sed -i -E $REGLA_REEMPLAZO $ARCHIVO
    sed -i 's/{{{SPACE}}}/ /g' $ARCHIVO
done