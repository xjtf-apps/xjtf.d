# we should be in xjtf/src/xjtf.d folder

pushd ../xjtf.d.ui
npm run build
Copy-Item -Path ".\dist\*" "..\xjtf.d\wwwroot" -Recurse
popd