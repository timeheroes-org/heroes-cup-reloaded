#!/bin/bash
#yset -x
sudo mysql heroes_cup_db --execute 'select FileName,Id from Images;' | while read image
do
echo $image
image_name=$(echo $image | awk '{FS=" "} {print $1}')
dir=$(echo $image | awk 'BEGIN {FS=" "} {print $2}')
mkdir -p /var/www/heroes-cup-static/$dir
chmod 777 /var/www/heroes-cup-static/$dir
echo "Dumping $dir/$image_name"
#sudo base64 $(mysql heroes_cup_db --execute "select to_base64(Bytes) from Images where Id='$image_name'") > "wwwroot/uploads/$image_name.$ext"

sudo mysql heroes_cup_db --execute "select Bytes from Images where FileName='$image_name' INTO DUMPFILE '/var/www/heroes-cup-static/$dir/$image_name'" 
sudo chown www-data /var/www/heroes-cup-static/ -R
done
