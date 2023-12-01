use godot::prelude::*;

mod components;
mod entities;
mod fsm;

/// GDExtension entry
struct RustExtension;

#[gdextension]
unsafe impl ExtensionLibrary for RustExtension {}
