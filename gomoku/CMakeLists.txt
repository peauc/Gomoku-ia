enable_language(CSharp)
project(gomoku)

cmake_minimum_required(VERSION 3.10)

file(GLOB_RECURSE SRC src/*.cs)

add_executable(gomoku ${SRC})