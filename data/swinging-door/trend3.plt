reset

#set terminal dumb

set grid
set title 'Swinging door compression -- Trend3'
set xlabel 'Time'
set ylabel 'Value'

set xrange [0:10]
set yrange [-3:6]

set style line 2 lc rgb 'green' pt 9	# triangle

#set datafile separator ";"

# replot is also possible for the second plot
plot    'trend3_raw.csv'         with linespoints title 'raw', \
        'trend3_compressed.csv'  with points ls 2 title 'compressed'
