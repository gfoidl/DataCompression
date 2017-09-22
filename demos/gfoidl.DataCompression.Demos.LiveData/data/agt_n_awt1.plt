reset

#set terminal dumb
set term pngcairo size 1200,600 enhanced
set output 'agt_n_awt1.png'

set grid
set title 'AGT n AWT1'
set xlabel 'Time'
set ylabel '[°C]'
#set key bottom right

set xdata time
set timefmt "%Y-%m-%d_%H:%M:%S"
set format x "%H:%M"

#set datafile separator ";"

# replot is also possible for the second plot
plot    'agt_n_awt1.csv'            using 1:2 with lines title 'raw', \
        'agt_n_awt1_compressed.csv' using 1:2 with lines title 'compressed'