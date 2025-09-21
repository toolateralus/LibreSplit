bear -- clang -std=c23 -L./ -lxserverinput -o tests tests.c
LD_LIBRARY_PATH="./" ./tests
rm tests