echo Start cleaning...
find . -name 'node_modules' | xargs -L1 rm -rvf
find . -name 'bin' | xargs -L1 rm -rvf
find . -name 'obj' | xargs -L1 rm -rvf
rm ./.vs -rvf
rm ./Kahla.App/www -rvf
rm ./Kahla.App/dist -rvf
rm ./CDN/dist -rvf
rm ./CDN/fonts -rvf
exit
