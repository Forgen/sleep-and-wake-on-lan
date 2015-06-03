Contains 2 projects:

WakeOnLan
Allows you to wake (sends magic packet) or sleep (sends "antimagic" packet) remote computer which supports Wake-on-LAN technology through internet.

SleepOnLan
Allows computer to receive "antimagic" packet from internet and do following actions: sleep, hibernate, reboot, shutdown, log off or lock your computer. Starting from build [r10](https://code.google.com/p/sleep-and-wake-on-lan/source/detail?r=10) you can choose action remotely, but it works only if your mac address begins with "00:".

Antimagic packet is the same as magic packet but order of mac address segments is reversed.