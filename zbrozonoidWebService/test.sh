
echo '>>>PUT<<<'

curl -v -d @request.json -H 'Content-Type: application/json' -X PUT http://zbrozonoid.bonobo.linuxpl.info/api/values/1

echo '>>>GET<<<'
curl -v http://zbrozonoid.bonobo.linuxpl.info/api/values/1

echo
