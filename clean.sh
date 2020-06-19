echo Start cleaning...
find . -name "bin"  -exec bash -c "rm -rvf '{}'" \;
find . -name "obj"  -exec bash -c "rm -rvf '{}'" \;
rm ./.vs -rvf
exit
