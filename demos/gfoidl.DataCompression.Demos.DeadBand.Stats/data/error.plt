reset

set term pngcairo size 1200,1000 enhanced
set output 'error-deadband.png'

set multiplot layout 4, 1 title "Error for dead band compression\n"

unset title
set grid
set key top right

set ytics 10.0
plot 'result.csv' using 1:2 with lines title 'raw'
plot 'compressed.csv' with dots title 'compressed'
plot 'result.csv' using 1:3 with lines title 'compressed reconstructed'

#set xlabel 'x'
set ytics 0.2
set yrange [-0.3:0.3]
plot 'result.csv' using 1:4 with lines title 'error'

unset multiplot
