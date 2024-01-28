FROM osrf/ros:humble-desktop

RUN  mkdir /ros_ws
WORKDIR /ros_ws

RUN apt update -y
RUN git clone https://github.com/foxglove/ros-foxglove-bridge.git src/ros-foxglove-bridge


ENV ROS_VERSION=2
RUN rosdep update
RUN rosdep install --ignore-src --default-yes --from-path src
RUN /bin/bash -c "source /opt/ros/humble/setup.bash && colcon build --event-handlers console_direct+ --symlink-install"
RUN echo 'source /ros_ws/install/local_setup.bash' >> ~/.bashrc 
RUN echo 'source /opt/ros/humble/setup.bash' >> ~/.bashrc 


EXPOSE 8765

ENTRYPOINT ["/bin/bash","-c","source /ros_ws/install/local_setup.bash && source /opt/ros/humble/setup.bash && ros2 launch foxglove_bridge foxglove_bridge_launch.xml" ]