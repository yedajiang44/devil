#!/usr/bin/env bash
cd output/linux-x64 || exit
./devil cpu consume -u 50 -t -1 &
pid[0]=$!
./devil ram consume -u 50 -t -1 &
pid[1]=$!
trap 'kill ${pid[0]} ${pid[1]}; exit 1' INT
wait
