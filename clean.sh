echo Start cleaning...
find . -name 'bin' | xargs -L1 rm -rvf
find . -name 'obj' | xargs -L1 rm -rvf
rm ./.vs -rvf
exit
