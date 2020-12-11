reset

#set terminal dumb

set grid
set title 'Swinging door compression -- maxDelta'
set xlabel 'Time'
set ylabel 'Value'
set key bottom right
set xrange [0:21]
set yrange [0:5]

#set datafile separator ";"

# replot is also possible for the second plot
plot    'maxDelta_raw.csv'          with linespoints title 'raw', \
        'maxDelta_compressed.csv'   with linespoints title 'compressed'
