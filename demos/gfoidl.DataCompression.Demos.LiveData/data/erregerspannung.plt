reset

#set terminal dumb
set term pngcairo size 1200,600 enhanced
set output 'erregerspannung.png'

set grid
set title 'Erregerspannung'
set xlabel 'Time'
set ylabel '[V]'
set key bottom right

set yrange [14:25]

set xdata time
set timefmt "%Y-%m-%d_%H:%M:%S"
set format x "%H:%M"

#set datafile separator ";"

# replot is also possible for the second plot
plot    'erregerspannung.csv'               using 1:2 with lines title 'raw', \
        'erregerspannung_compressed.csv'    using 1:2 with lines title 'compressed'