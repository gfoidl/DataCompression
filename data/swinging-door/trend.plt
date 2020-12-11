reset

#set terminal dumb

set grid
set title 'Swinging door compression -- Trend'
set xlabel 'Time'
set ylabel 'Value'

set xrange [0:17]
set yrange [0:6]

#set datafile separator ";"

# replot is also possible for the second plot
plot    'trend_raw.csv'         with linespoints title 'raw', \
        'trend_compressed.csv'  with linespoints title 'compressed'
