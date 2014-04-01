package ru.vint.labsoo;

import android.support.v7.app.ActionBarActivity;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.AdapterView.OnItemClickListener;
import android.util.Log;
import android.content.Intent;
import ru.vint.nightjar.R;

public class List extends ActionBarActivity {

	public final static String THIEF = "ru.vint.Lab1.THIEF";
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_list);

		if (savedInstanceState == null) {
			getSupportFragmentManager().beginTransaction()
					.add(R.id.container, new PlaceholderFragment()).commit();
		}
	}
	
	@Override
	protected void onResume() 
	{
	    super.onResume();
	  
		// находим список
	    ListView lvMain = (ListView) findViewById(R.id.listView1);
	    lvMain.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
	    String[] names = getIntent().getStringArrayExtra("Scripts");
	    // создаем адаптер
	    ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,
	        android.R.layout.simple_list_item_1, names);

	    // присваиваем адаптер списку
	    lvMain.setAdapter(adapter);
	    
	    lvMain.setOnItemClickListener
	    (
	    		new OnItemClickListener() 
	    		{
	    			public void onItemClick(AdapterView<?> parent, View view,
	            	int position, long id) 
	    			{
	    				Intent answerInent = new Intent();
	    				answerInent.putExtra(THIEF, position);
	    				Log.d("LIST", "itemClick: position = " + position);
	    				setResult(RESULT_OK, answerInent);
	    				finish();
	    			}
	    		}
	    );
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {

		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.list, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_settings) {
			return true;
		}
		return super.onOptionsItemSelected(item);
	}

	/**
	 * A placeholder fragment containing a simple view.
	 */
	public static class PlaceholderFragment extends Fragment {

		public PlaceholderFragment() {
		}

		@Override
		public View onCreateView(LayoutInflater inflater, ViewGroup container,
				Bundle savedInstanceState) {
			View rootView = inflater.inflate(R.layout.list, container, false);
			return rootView;
		}
	}

}
