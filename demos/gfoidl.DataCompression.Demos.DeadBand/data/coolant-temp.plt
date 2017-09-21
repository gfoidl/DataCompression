reset

#set terminal dumb
set term pngcairo size 800,600 enhanced
set output 'coolant-temp.png'

set grid
set title 'Coolant temp over Oh'
set xlabel 'Oh'
set ylabel 'temp [°C]'
set key bottom right

#set datafile separator ";"

# replot is also possible for the second plot
plot    'coolant-temp.csv'              with linespoints title 'raw', \
        'coolant-temp_compressed.csv'   with linespoints title 'compressed'