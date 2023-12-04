#![windows_subsystem = "windows"]
use godot::prelude::*;

mod components;
mod entities;
mod fsm;
mod pid_camera;

/// GDExtension entry
struct RustExtension;

#[gdextension]
unsafe impl ExtensionLibrary for RustExtension {}
