echo Your container args are: "$@"
chmod +x /root/build/linuxBuild.x86_64
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24:32' /root/build/UnityMmo.x86_64 -batchmode -nographics $@