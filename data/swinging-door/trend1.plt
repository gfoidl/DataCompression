reset

# set terminal dumb
set term pngcairo size 800,600 enhanced
set output "trend1.png"

set grid
set title "Swinging door compression -- Trend1"
set xlabel "Time"
set ylabel "Value"

set xrange [0:7.5]
set yrange [0:2.5]

set style line 2 lc rgb "green" pt 9    # triangle
set style line 3 lc rgb "red"   pt 6    # circle

#set datafile separator ";"

# replot is also possible for the second plot
plot    "trend1_raw.csv"         with linespoints      title "raw", \
        "trend1_compressed.csv"  with points ls 2 ps 2 title "expected", \
        "trend1_actual.csv"      with points ls 3 ps 2 title "actual"
