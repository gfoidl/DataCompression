reset

#set terminal dumb
set term pngcairo size 1200,600 enhanced
set output 'agt_zyl_6.png'

set grid
set title 'AGT Zyl. 6'
set xlabel 'Time'
set ylabel '[°C]'
#set key bottom right

#set yrange [14:25]

set xdata time
set timefmt "%Y-%m-%d_%H:%M:%S"
set format x "%H:%M"

#set datafile separator ";"

# replot is also possible for the second plot
plot    'agt_zyl_6.csv'             using 1:2 with lines title 'raw', \
        'agt_zyl_6_compressed.csv'  using 1:2 with lines title 'compressed'