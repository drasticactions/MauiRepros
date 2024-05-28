//
//  ContentView.swift
//  AppleColorsSwiftUI
//
//  Created by DrasticActions on 2024/05/28.
//

import SwiftUI

struct ContentView: View {
    var body: some View {
        VStack(spacing: 20) {
            Button("⬛︎⬛︎⬛︎⬛︎⬛︎ Enabled SwiftUI ⬛︎⬛︎⬛︎⬛︎⬛︎⬛︎") {
                // Action for the enabled button
            }
            
            Button("⬛︎⬛︎⬛︎⬛︎⬛︎ Disabled SwiftUI ⬛︎⬛︎⬛︎⬛︎⬛︎⬛︎") {
                // Action for the disabled button (won't be triggered)
            }
            .disabled(true) // Disable this button
        }
        .padding()
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
