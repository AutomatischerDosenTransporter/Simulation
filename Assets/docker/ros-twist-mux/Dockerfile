FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install ros-foxy-twist-mux -y

ENTRYPOINT ["/ros_entrypoint.sh"]