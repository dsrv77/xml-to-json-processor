#!/bin/bash

URL="https://localhost:5000/xml-to-json"

FILE1="test-food.xml"
FILE2="test-catalog.xml"
FILE3="large.xml"
FILE4="empty.xml"
FILE5="wrong-ext.mlx"
FILE6="error-xml.xml"

curl -k -X POST -F "file=@$FILE1" -F "filename=large.xml" $URL &
curl -k -X POST -F "file=@$FILE2" -F "filename=test-catalog.xml" $URL &
curl -k -X POST -F "file=@$FILE3" -F "filename=test-food.xml" $URL &
curl -k -X POST -F "file=@$FILE4" -F "filename=empty.xml" $URL &
curl -k -X POST -F "file=@$FILE5" -F "filename=wrong-ext.mlx" $URL &
curl -k -X POST -F "file=@$FILE6" -F "filename=error-xml.xml" $URL &

wait

echo "All uploads are done!"