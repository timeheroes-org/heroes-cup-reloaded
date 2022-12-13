#!/bin/bash
#yset -x
sudo mysql heroes_cup_db --execute 'select FileName,ContentType from Images;' | while read image
do
image_name=$(echo $image | awk '{print $1}')

echo "Dumping $image_name"
#sudo base64 $(mysql heroes_cup_db --execute "select to_base64(Bytes) from Images where Id='$image_name'") > "wwwroot/uploads/$image_name.$ext"
mkdir -p /tmp/uploads
sudo mysql heroes_cup_db --execute "select Bytes from Images where FileName='$image_name' INTO DUMPFILE '/tmp/uploads/$image_name'" 
sudo chown andrey /tmp/uploads/ -R
done
