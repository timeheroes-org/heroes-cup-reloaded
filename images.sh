#!/bin/bash
#yset -x
sudo mysql heroes_cup_db --execute 'select Id,ContentType from Images;' | while read image
do
image_name=$(echo $image | awk '{print $1}')
content_type=$(echo $image | awk '{print $2}')
ext="jpg"
if [ "$content_type" = "image/png" ]; then 
ext="png"
elif [ "$content_type" = "application/pdf" ]; then 
ext="pdf"
fi
echo "Dumping $image_name.$ext"
#sudo base64 $(mysql heroes_cup_db --execute "select to_base64(Bytes) from Images where Id='$image_name'") > "wwwroot/uploads/$image_name.$ext"
mkdir -p /tmp/uploads
sudo mysql heroes_cup_db --execute "select Bytes from Images where Id='$image_name' INTO DUMPFILE '/tmp/uploads/$image_name.$ext'" 
sudo chown andrey /tmp/uploads/ -R
done
