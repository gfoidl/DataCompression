reset

#set terminal dumb

set grid
set title 'Swinging door compression -- Trend3 (minimal repro)'
set xlabel 'Time'
set ylabel 'Value'

set xrange [4:10]
set yrange [-3:6]

set style line 2 lc rgb 'green' pt 9	# triangle

#set datafile separator ";"

# replot is also possible for the second plot
plot    'trend3_mini_raw.csv'         with linespoints title 'raw', \
        'trend3_mini_compressed.csv'  with points ls 2 title 'compressed'
