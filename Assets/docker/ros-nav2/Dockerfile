FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install ros-foxy-navigation2 -y
RUN apt install ros-foxy-nav2-bringup -y

ENTRYPOINT ["/ros_entrypoint.sh"]